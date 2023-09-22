using UnityEngine;

public abstract class IAbilityImplementation : MonoBehaviour
{
    public abstract void StartingSkill((AbilityDataSerialize serialized, BodyPartAbility ability) actionData, PlayerAbilityController abilityControllerRef);

}
