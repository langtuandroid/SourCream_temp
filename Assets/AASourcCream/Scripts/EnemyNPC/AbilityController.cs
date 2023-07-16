using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AbilityController : SerializedMonoBehaviour
{
    [FolderPath]
    public string dataLocation;

    public Dictionary<string, GameObject> attackColliders;
    public Dictionary<string, GameObject> attackIndicators;

    private bool collisionOccured = false;

    private EnemyCombatAction currentCombatAction;

    public bool abilityInProgress = false;
    //this is supposed to be adaptable rather than hardset
    public FighterController fighterController;
    //TODO this shouldn't be here and instead be in archType classes like Fighter controller etc.
    public StatsComponent statsComponent;


    // Start is called before the first frame update
    void Start()
    {
        fighterController = gameObject.GetComponent<FighterController>();
        statsComponent = gameObject.GetComponent<StatsComponent>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void callCombatAction(EnemyCombatAction actionData, GameObject player)
    {
        currentCombatAction = actionData;
        abilityInProgress = true;
        if (actionData.target == TargetTypes.ALLY || actionData.target == TargetTypes.SELF) {
            switch (actionData.target) {
                case TargetTypes.ALLY:
                    buffAlly(actionData);
                    break;
                case TargetTypes.SELF:
                    buffSelf(actionData);
                    break;
                default:
                    break;
            }

        } else {
            switch (actionData.attackType) {
                case AttackType.INSTANT:
                    instantAttack(actionData, player);
                    break;
                case AttackType.PROJECTILE:
                    projectileAttack(actionData, player);
                    break;
                case AttackType.RANGEINSTANT:
                    instantRangeAttack(actionData, player);
                    break;
                case AttackType.CHANNEL:
                    channelAttack(actionData, player);
                    break;
                default:
                    break;
            }
        }

    }

    public GameObject getIndicator(string abilityName)
    {
        return attackIndicators?[abilityName];
    }

    public GameObject getCollider(string abilityName)
    {
        return attackColliders?[abilityName];
    }

    public void instantAttack(EnemyCombatAction actionData, GameObject player)
    {
        var indicator = getIndicator(actionData.name);
        var calliderToUse = getCollider(actionData.name);

        Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        collisionOccured = false;
        //IF INDICATOR EXISTS SPAWN INDICATOR  FOR CAST TIME
        if (indicator != null) {
            //FUCK THIS FUCKIN PILE OF SHIT FUCKIN ROTATION
            GameObject goIndicator = (GameObject)GameObject.Instantiate(indicator, transform.position, rotation);
            goIndicator.transform.parent = gameObject.transform;
            StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(actionData.castTime, goIndicator));
        }

        //SPAWN COLLIDER AFTER CAST TIME
        if (calliderToUse != null) {
            CallMethodWithDelay(actionData.castTime, () => {
                GameObject goCollider = (GameObject)GameObject.Instantiate(calliderToUse, transform.position, rotation);
                SetupCollisionDetection(actionData, goCollider, player);

                goCollider.transform.parent = gameObject.transform;
                // TODO FIGURE OUT IF THIS VALUE CAN BE MINIMAL LIKE 0.1s
                StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(0.3f, goCollider));
                abilityFinished();
            });
        }
    }
    public void projectileAttack(EnemyCombatAction actionData, GameObject player)
    {
        var indicator = getIndicator(actionData.name);
        var colliderToUse = getCollider(actionData.name);

        Vector3 indicatorDirection = new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z).normalized;
        Quaternion indicatorRotation = Quaternion.LookRotation(indicatorDirection, Vector3.up);

        if (indicator != null) {
            GameObject goIndicator = (GameObject)GameObject.Instantiate(indicator, transform.position, indicatorRotation);
            goIndicator.transform.parent = gameObject.transform;
            StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(actionData.castTime, goIndicator));
        }

        if (colliderToUse != null) {
            //TODO spawner should be setable i.e bow for an arrow 
            var spawnerLocation = new Vector3(transform.position.x + 1.0f, transform.position.y + 1.0f, transform.position.z);
            Vector3 direction = player.transform.position - spawnerLocation;
            CallMethodWithDelay(actionData.castTime, () => {
                GameObject currentProjectile = (GameObject)GameObject.Instantiate(colliderToUse, spawnerLocation, Quaternion.identity);

                SetupCollisionDetection(actionData, currentProjectile, player);

                currentProjectile.transform.forward = direction.normalized;
                //TODO: unsure when to destroy the projectile might need a function to check when it hits terrain/walls or reaches max range 
                abilityFinished();

                var position = player.transform.position;
                var direction2 = (player.transform.position - spawnerLocation).normalized;
                StartCoroutine(MoveTowardsTarget2(5.0f, direction2, 4.9f, currentProjectile));
                //StartCoroutine(MoveTowardsTarget(position, 10f, currentProjectile, 4.9f, false));
                if (currentProjectile) {
                    StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(5.0f, currentProjectile));
                }

            });
        }
    }

    public void buffSelf(EnemyCombatAction actionData)
    {
        Debug.Log("BUFF SELLF");
        StartCoroutine(CallMethodForDuration(actionData.castTime, (actionData.castTime / 2), () => statsComponent.Heal(100)));
        CallMethodWithDelay(actionData.castTime + 0.2f, () => abilityFinished());
    }

    public void buffAlly(EnemyCombatAction actionData)
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


    private void abilityFinished()
    {
        Debug.Log("ABILITY FINISHED");
        var shouldReset = false;
        abilityInProgress = false;
        if (statsComponent.health.currentHealth < (statsComponent.health.maxHealth / 2)) {
            fighterController.UpdateActionTypeWeights(Actions.BUFF, 1000);
            shouldReset = true;
        } else {
            fighterController.UpdateActionTypeWeights(Actions.ATTACK, 10000);
            shouldReset = true;
        }
        Debug.Log(fighterController.actionTypeWeightedList.GetWeightAtIndex(0));
        Debug.Log(fighterController.actionTypeWeightedList.GetWeightAtIndex(1));
        fighterController.setNextPreferedAction(shouldReset, false);
    }

    private void SetupCollisionDetection(EnemyCombatAction actionData, GameObject colliderToUse, GameObject target)
    {
        CollisionDetection collisionDetection = colliderToUse.GetComponent<CollisionDetection>();
        collisionDetection.statsComponent = target.GetComponent<StatsComponent>();
        collisionDetection.SetDamageInfo(new DamageInformation(actionData.scallingType, actionData.value));
    }

    private IEnumerator MoveTowardsTarget(Vector3 target, float speed, GameObject particle, float duration, bool chase)
    {
        //isMoving = true;

        float timer = 0f;
        var setTarget = target;

        while (timer < duration) {
            if (target != null) {
                var target2 = chase ? target : setTarget;
                // Calculate the direction to the target location
                Vector3 direction = target2 - particle.transform.position;

                // Normalize the direction vector to get a consistent speed
                direction.Normalize();

                // Move towards the target location based on the speed and deltaTime
                particle.transform.position += direction * speed * Time.deltaTime;

            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveTowardsTarget2(float speed, Vector3 direction, float duration, GameObject particle)
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

    public void instantRangeAttack(EnemyCombatAction actionData, GameObject target)
    {

    }
    public void channelAttack(EnemyCombatAction actionData, GameObject target)
    {

    }


    public void callBuffAction(EnemyCombatAction actionData)
    {

    }

    public void CallMethodWithDelay(float delayInSeconds, System.Action methodToCall)
    {
        StartCoroutine(DelayedMethod(delayInSeconds, methodToCall));
    }

    // private void OnTriggerEnter(Collider collider)
    // {
    //     Debug.Log("HUHUHUHUHUHUHHUHUH");

    //     if (collider.transform.gameObject.layer == 7) {
    //         collisionOccured = true;

    //         statsComponent.Damage(new DamageInformation(currentCombatAction.scallingType, currentCombatAction.value));
    //         //Disabling to not do damage twice continuos dots might need a change here
    //         collider.isTrigger = false;
    //     }
    // }

    // private void OnCollisionEnter(Collision other)
    // {
    //     Debug.Log("WHAT");
    // }

    // private void OnCollisionExit(Collision collider)
    // {
    //     if (collider.transform.gameObject.layer == 7 && collisionOccured == true) {
    //         collisionOccured = false;
    //     }
    // }

    private IEnumerator DelayedMethod(float delayInSeconds, System.Action methodToCall)
    {
        yield return new WaitForSeconds(delayInSeconds);
        methodToCall.Invoke();
    }
}
