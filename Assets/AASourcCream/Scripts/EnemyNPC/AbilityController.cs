using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class AbilityController : SerializedMonoBehaviour
{
    public Dictionary<string, GameObject> attackColliders;
    public Dictionary<string, GameObject> attackIndicators;

    private bool collisionOccured = false;

    private EnemyCombatAction currentCombatAction;

    public bool abilityInProgress = false;
    //this is supposed to be adaptable rather than hardset
    public FighterController fighterController;
    //TODO this shouldn't be here and instead be in archType classes like Fighter controller etc.
    public StatsComponent statsComponent;

    private EnemyAnimationCtrl animController;


    // Start is called before the first frame update
    void Start()
    {
        fighterController = gameObject.GetComponent<FighterController>();
        statsComponent = gameObject.GetComponent<StatsComponent>();
        animController = this.GetComponent<EnemyAnimationCtrl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CallCombatAction(EnemyCombatAction actionData, GameObject player)
    {
        Debug.Log(actionData.name);
        currentCombatAction = actionData;
        abilityInProgress = true;
        if (actionData.target == TargetTypes.ALLY || actionData.target == TargetTypes.SELF) {
            switch (actionData.target) {
                case TargetTypes.ALLY:
                    BuffAlly(actionData);
                    break;
                case TargetTypes.SELF:
                    BuffSelf(actionData);
                    break;
                default:
                    break;
            }

        } else {
            switch (actionData.attackType) {
                case AttackType.INSTANT:
                    InstantAttack(actionData, player);
                    break;
                case AttackType.PROJECTILE:
                    ProjectileAttack(actionData, player);
                    break;
                case AttackType.RANGEINSTANT:
                    InstantRangeAttack(actionData, player);
                    break;
                case AttackType.CHANNEL:
                    ChannelAttack(actionData, player);
                    break;
                default:
                    break;
            }
        }
    }

    public GameObject GetIndicator(string abilityName)
    {
        if (attackIndicators.ContainsKey(abilityName)) {
            return attackIndicators?[abilityName];
        } else {
            return null;
        }
    }

    public GameObject GetCollider(string abilityName)
    {
        if (attackColliders.ContainsKey(abilityName)) {
            return attackColliders?[abilityName];
        } else {
            return null;
        }
    }

    public void InstantAttack(EnemyCombatAction actionData, GameObject player)
    {
        var indicator = GetIndicator(actionData.name);
        var colliderToUser = GetCollider(actionData.name);

        Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z).normalized;
        //Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        collisionOccured = false;
        //IF INDICATOR EXISTS SPAWN INDICATOR  FOR CAST TIME
        if (indicator != null) {
            //FUCK THIS FUCKIN PILE OF SHIT FUCKIN ROTATION
            GameObject goIndicator = (GameObject)GameObject.Instantiate(indicator, transform.position, transform.rotation);
            goIndicator.transform.parent = gameObject.transform;
            StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(actionData.castTime, goIndicator));
        }

        //Start animation
        animController.SetAnimatorVar(actionData.name, true);

        //SPAWN COLLIDER AFTER CAST TIME
        if (colliderToUser != null) {
            CallMethodWithDelay(actionData.castTime * 0.8f, () => {
                GameObject goCollider = (GameObject)GameObject.Instantiate(colliderToUser, transform.position, transform.rotation);
                SetupCollisionDetection(actionData, goCollider, player);

                goCollider.transform.parent = gameObject.transform;
                AbilityFinished(actionData?.name);

                // TODO FIGURE OUT IF THIS VALUE CAN BE MINIMAL LIKE 0.1s
                StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(actionData.castTime - (actionData.castTime * 0.8f) - 0.15f, goCollider));
            });
        }
    }
    public void ProjectileAttack(EnemyCombatAction actionData, GameObject player)
    {
        var indicator = GetIndicator(actionData.name);
        var colliderToUse = GetCollider(actionData.name);

        Vector3 indicatorDirection = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z).normalized;
        Quaternion indicatorRotation = Quaternion.LookRotation(indicatorDirection, Vector3.up);

        if (indicator != null) {
            GameObject goIndicator = (GameObject)GameObject.Instantiate(indicator, transform.position, indicatorRotation);
            goIndicator.transform.parent = gameObject.transform;
            StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(actionData.castTime, goIndicator));
        }

        if (colliderToUse != null) {
            //TODO spawner should be setable i.e bow for an arrow 
            var spawnerLocation = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z);
            Vector3 direction = player.transform.position - spawnerLocation;

            //Start animation
            animController.SetAnimatorVar(actionData.name, true);

            CallMethodWithDelay(actionData.castTime * 0.4f, () => {
                GameObject currentProjectile = (GameObject)GameObject.Instantiate(colliderToUse, spawnerLocation, Quaternion.identity);

                SetupCollisionDetection(actionData, currentProjectile, player);

                currentProjectile.transform.forward = direction.normalized;


                var position = player.transform.position;
                var direction2 = (player.transform.position - spawnerLocation).normalized;
                StartCoroutine(MoveTowardsTarget(12.0f, direction2, 4.9f, currentProjectile));
                if (currentProjectile) {
                    StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(5.0f, currentProjectile));
                }

            });

            CallMethodWithDelay(actionData.castTime, () => {
                //TODO: unsure when to destroy the projectile might need a function to check when it hits terrain/walls or reaches max range 
                AbilityFinished(actionData?.name);
            });
        }
    }

    public void BuffSelf(EnemyCombatAction actionData)
    {
        Debug.Log("BUFF SELLF");
        StartCoroutine(CallMethodForDuration(actionData.castTime, (actionData.castTime / 2), () => statsComponent.Heal(100)));
        CallMethodWithDelay(actionData.castTime + 0.2f, () => AbilityFinished(actionData?.name));
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


    private void AbilityFinished(string abilityName)
    {
        animController.SetAnimatorVar(abilityName, false);
        fighterController.OnAbilityFinished();
        abilityInProgress = false;
    }

    private void SetupCollisionDetection(EnemyCombatAction actionData, GameObject colliderToUse, GameObject target)
    {
        CollisionDetection collisionDetection = colliderToUse.GetComponent<CollisionDetection>();
        collisionDetection.statsComponent = target.GetComponent<StatsComponent>();
        collisionDetection.SetDamageInfo(new DamageInformation(actionData.scallingType, actionData.value));
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
}
