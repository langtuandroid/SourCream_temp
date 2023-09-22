using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    // Start is called before the first frame update
    public Dictionary<string, GameObject> attackColliders;
    public Dictionary<string, GameObject> attackIndicators;

    public (AbilityDataSerialize serialized, BodyPartAbility ability) currentCombatAction;

    public bool abilityInProgress = false;
    //this is supposed to be adaptable rather than hardset
    //TODO this shouldn't be here and instead be in archType classes like Fighter controller etc.
    public StatsComponent statsComponent;

    public PlayerDataController dataController;

    [SerializeField]
    private PlayerUI playerUi;

    public CooldownManager cooldownManger;


    // Start is called before the first frame update
    void Start()
    {
        statsComponent = gameObject.GetComponent<StatsComponent>();
        dataController = gameObject.GetComponent<PlayerDataController>();
        cooldownManger = gameObject.GetComponent<CooldownManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CallAbility(int index)
    {
        var abilityData = dataController.GetAbilityTuple(index);
        Debug.Log("asdasdas");
        CallCombatAction(abilityData);
    }

    public void CallCombatAction((AbilityDataSerialize serialized, BodyPartAbility ability) actionData)
    {
        if (actionData.serialized.abilityImplementation != null) {
            Debug.Log("going into custom");
            actionData.serialized.abilityImplementation.StartingSkill(actionData, this);
            return;
        }
        if (cooldownManger.IsCooldownRunning(actionData.ability.name) || abilityInProgress) {
            return;
        }

        currentCombatAction = actionData;
        abilityInProgress = true;


        switch (actionData.ability.attackType) {
            case AttackType.INSTANT:
                InstantAttack(actionData);
                break;
            case AttackType.PROJECTILE:
                ProjectileAttack(actionData);
                break;
            case AttackType.RANGEINSTANT:
                //InstantRangeAttack(actionData, player);
                break;
            case AttackType.CHANNEL:
                //ChannelAttack(actionData, player);
                break;
            default:
                break;
        }
    }

    public GameObject GetIndicator(string abilityName)
    {
        return attackIndicators?[abilityName];
    }

    public GameObject GetCollider(string abilityName)
    {
        return attackColliders?[abilityName];
    }

    public Quaternion GetCursorPos()
    {
        // Get the mouse position in screen coordinates
        Vector3 mouseScreenPos = Input.mousePosition;

        // Convert the mouse position from screen coordinates to world coordinates
        mouseScreenPos.z = -Camera.main.transform.position.z; // Distance from the camera to the game objects (assuming the camera is orthographic)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // Calculate the direction from the current position to the mouse position
        Vector3 directionToMouse = mouseWorldPos - transform.position;

        // Calculate the angle in degrees between the forward direction of the object and the direction to the mouse
        float angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // Create a Quaternion to represent the rotation around the Y (up) axis
        Quaternion rotation = Quaternion.Euler(0f, angleToMouse, 0f);

        // Instantiate the game object at the specified position (x) with the correct rotation
        return rotation;
    }

    public void InstantAttack((AbilityDataSerialize serialized, BodyPartAbility ability) actionData)
    {
        var calliderToUse = actionData.serialized?.Collider;
        var Vfx = actionData.serialized?.VFX;

        //Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z).normalized;
        Quaternion floatVal = GetCursorPos();

        //SPAWN COLLIDER AFTER CAST TIME
        if (calliderToUse != null) {
            CallMethodWithDelay(actionData.ability.castTime, () => {

                GameObject goCollider = (GameObject)GameObject.Instantiate(calliderToUse, transform.position, transform.rotation);
                GameObject spawnedVfx = (GameObject)GameObject.Instantiate(Vfx, transform.position, transform.rotation);
                goCollider.transform.parent = gameObject.transform;
                spawnedVfx.transform.parent = gameObject.transform;

                StartCooldown(actionData.ability.name, actionData.ability.cooldown);
                SetupCollisionDetection(actionData.ability.powerScaling, goCollider);

                // TODO FIGURE OUT IF THIS VALUE CAN BE MINIMAL LIKE 0.1s
                StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(0.1f, goCollider));
                StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(0.6f, spawnedVfx));
                AbilityFinished();
            });
        }
    }



    public void ProjectileAttack((AbilityDataSerialize serialized, BodyPartAbility ability) actionData)
    {
        var indicator = actionData.serialized?.Indicator;
        var colliderToUse = actionData.serialized?.Collider;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f; // Distance from the camera to the GameObject

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (colliderToUse != null) {
            //TODO spawner should be setable i.e bow for an arrow 
            var spawnerLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.0f);
            Vector3 direction = mousePosition - spawnerLocation;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Terrain"));

            CallMethodWithDelay(actionData.ability.castTime, () => {

                GameObject currentProjectile = (GameObject)GameObject.Instantiate(colliderToUse, spawnerLocation, transform.rotation);
                Vector3 targetPosition = new Vector3(hit.point.x, hit.point.y + 1.0f, hit.point.z);
                StartCooldown(actionData.ability.name, actionData.ability.cooldown);

                SetupCollisionDetection(actionData.ability.powerScaling, currentProjectile);

                //TODO: unsure when to destroy the projectile might need a function to check when it hits terrain/walls or reaches max range 
                AbilityFinished();
                Vector3 pushDirection = (targetPosition - transform.position).normalized;
                StartCoroutine(MoveTowardsTarget(10.0f, pushDirection, 4.9f, currentProjectile));
                if (currentProjectile) {
                    StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(5.0f, currentProjectile));
                }

            });
        }
    }

    //TODO take in ID instead of name
    public void StartCooldown(string name, float cooldown)
    {
        //I think should pass cd here as well
        Debug.Log(name);
        playerUi.CallSkill(name);
        cooldownManger.StartCooldown(name, cooldown);
    }

    public void BuffSelf(EnemyCombatAction actionData)
    {
        Debug.Log("BUFF SELLF");
        StartCoroutine(CallMethodForDuration(actionData.castTime, (actionData.castTime / 2), () => statsComponent.Heal(100)));
        CallMethodWithDelay(actionData.castTime + 0.2f, () => AbilityFinished());
    }

    public void BuffAlly(EnemyCombatAction actionData)
    {
        // TODO IMPLEMENT
    }


    private IEnumerator CallMethodForDuration(float duration, float interval, Action callback)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            // Call your method here
            callback();

            yield return new WaitForSeconds(interval);

            elapsedTime += interval;
        }

        // Coroutine finished, perform any necessary clean-up or actions
        // This is where you can handle what happens after the desired duration
    }


    public void AbilityFinished()
    {
        Debug.Log("ABILITY FINISHED");
        abilityInProgress = false;
    }

    public CollisionDetection SetupCollisionDetection(float value, GameObject colliderToUse, Action<GameObject> optionalCallback = null)
    {
        CollisionDetection collisionDetection = colliderToUse.GetComponent<CollisionDetection>();
        collisionDetection.caster = Caster.PLAYER;
        collisionDetection.SetOnCollide(optionalCallback);
        collisionDetection.SetDamageInfo(new DamageInformation(ScalingTypes.PHYSICAL, value));
        return collisionDetection;
    }

    private IEnumerator MoveTowardsTarget(float speed, Vector3 direction, float duration, GameObject particle)
    {
        float timer = 0f;
        while (timer < duration) {
            if (particle) {
                particle.transform.position += direction * speed * Time.deltaTime;
            } else {
                yield break;
            }
            yield return null;
        }
        timer += Time.deltaTime;
        yield return null;
    }

    public void InstantRangeAttack(EnemyCombatAction actionData, GameObject target)
    {

    }
    public void ChannelAttack(EnemyCombatAction actionData, GameObject target)
    {

    }


    public void CallBuffAction(EnemyCombatAction actionData)
    {

    }

    public void CallMethodWithDelay(float delayInSeconds, System.Action methodToCall)
    {
        StartCoroutine(DelayedMethod(delayInSeconds, methodToCall));
    }

    private IEnumerator DelayedMethod(float delayInSeconds, System.Action methodToCall)
    {

        yield return new WaitForSeconds(delayInSeconds);
        methodToCall.Invoke();
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 1000f);
    }

}


public class BodyPartItem
{
    public string id;
    public string name;
    public float power;
    public float health;
    public float movementSpeed;
    public float attackSpeed;
    public int level;
    public SkillTree skillTree;
}

public class BodyPartAbility
{
    public string id;
    public string name;
    public float powerScaling;
    public AttackType attackType;
    public CastType castType;
    public float castTime;
    public float cooldown;
    public float range;
    public bool unlocked;
    public SkillTree skillTree;
    public BodyPartAbilityModifier[] modifiers;

    public bool? onCooldown;

}
public class SkillTree
{
    public BodyPartAbility[] abilities;
    //public BodyPartPassive[] passives;
}

public class BodyPartAbilityModifier
{
    public string id;
    public string name;
    public TriggerType triggerType;

    public ModifierType modifierType;
    public string trigger;
    public ModifierTargetTypes target;
    public StatType stat;
    public float value;
    public float time;
    public BodyPartAbilityModifier[] modifiers;
}

public class Modfier
{
    public string name;
    public ModifierType modifierType;
}

public class ModifierType
{
    public string name;
    public ModifierTypeEnum type;
    public float? stackAmoung;
    public float? stackTime;
}

public enum ModifierTargetTypes
{
    ENEMY,
    ALLY,
    SELF,
    LOCATION,
    ABILITY,
}

public enum ModifierTypeEnum
{
    STACK,
    OVERTIME,
    INSTANT
}

public enum StatType
{
    HEALTH,
    RESISTANCE,
    POWER,
    MOVEMENTSPEED,
    ATTACKSPEED,
    RANGE,
}

public enum TriggerType
{
    MODIFIER,
    ABILITY
}

