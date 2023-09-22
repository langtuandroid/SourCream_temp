using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneBlade_Slash : IAbilityImplementation
{
    [SerializeField]
    public GameObject mainAttack_Collider;
    [SerializeField]
    public GameObject mainAttack_VFX;

    [SerializeField]
    public GameObject bloodSplash_Collider;
    [SerializeField]
    public GameObject bloodSplash_VFX;

    PlayerAbilityController ac;
    (AbilityDataSerialize serialized, BodyPartAbility ability) abilityData;


    public override void StartingSkill((AbilityDataSerialize serialized, BodyPartAbility ability) actionData, PlayerAbilityController abilityControllerRef)
    {
        ac = abilityControllerRef;
        abilityData = actionData;

        if (ac.cooldownManger.IsCooldownRunning(actionData.ability.name) || ac.abilityInProgress) {
            return;
        }
        ac.abilityInProgress = true;
        ac.currentCombatAction = actionData;
        MainAttack(actionData);
    }

    public void MainAttack((AbilityDataSerialize serialized, BodyPartAbility ability) actionData)
    {
        var colliderToUse = mainAttack_Collider;
        var vfxToUse = mainAttack_VFX;

        Quaternion cursorPos = ac.GetCursorPos();
        Debug.Log("DEEEP");

        if (colliderToUse != null) {
            ac.CallMethodWithDelay(actionData.ability.castTime, () => {
                GameObject colliderObj = GameObject.Instantiate(colliderToUse, ac.gameObject.transform.position, ac.gameObject.transform.rotation);
                GameObject vfxObject = GameObject.Instantiate(vfxToUse, ac.gameObject.transform.position, ac.gameObject.transform.rotation);

                ac.StartCooldown(actionData.ability.name, actionData.ability.cooldown);
                var collision = ac.SetupCollisionDetection(actionData.ability.powerScaling, colliderObj, (gObject) => MainAttackOnHit(gObject));

                StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(0.1f, colliderObj));

                //Maybe get the particle system for these components and then check for duration instead of this manual set
                StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(2.1f, vfxObject));
                ac.AbilityFinished();

            });
        }
    }

    public void MainAttackOnHit(GameObject gObject)
    {
        Debug.Log(gObject.GetComponent<StatsComponent>().health);
        var colliderToUse = bloodSplash_Collider;
        var vfxToUse = bloodSplash_VFX;

        Quaternion cursorPos = ac.GetCursorPos();
        Debug.Log("DEEEP");

        if (colliderToUse != null) {
            ac.CallMethodWithDelay(abilityData.ability.castTime, () => {
                GameObject colliderObj = GameObject.Instantiate(colliderToUse, gObject.gameObject.transform.position, ac.gameObject.transform.rotation);
                GameObject vfxObject = GameObject.Instantiate(vfxToUse, gObject.gameObject.transform.position, ac.gameObject.transform.rotation);

                ac.StartCooldown(abilityData.ability.name, abilityData.ability.cooldown);
                var collision = ac.SetupCollisionDetection(abilityData.ability.powerScaling, colliderObj);
                Debug.Log("zzzzzzzzzzzzzzzzzzzzzzzzzz");

                StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(0.1f, colliderObj));
                StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(2.1f, vfxObject));
            });
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("asd");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
