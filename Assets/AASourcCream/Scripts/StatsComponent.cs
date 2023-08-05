using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;


public class StatsComponent : MonoBehaviour
{
    [SerializeField]
    public Health health = new Health();

    [SerializeField]
    private Dictionary<string, StatModifier> modifiers;

    public DamageNumber damageNumber;

    [SerializeField]
    public bool isPlayer = false;

    private float armor = 10.0f;

    public float attackDamge = 20.0f;

    public float magicDamage = 15.0f;

    [InspectorButton("ApplyModifier")]
    public bool applyModifier;

    public void ApplyModifier()
    {
        Debug.Log("Clicked");
        var mod = new StatModifier(StatModifierType.HEALTH, 1.0f);
        mod.AddToModifier(0.5f, 5);
    }

    public void Damage(DamageInformation dmgInfo)
    {
        var amount = CalculateDamage(dmgInfo);
        damageNumber.enableCombination = true;
        //This is reversed because the owner of stats component technically does damage to itself
        if (!isPlayer) {
            damageNumber.SetColor(Color.red);
            damageNumber.Spawn(transform.position, amount);
        }
        health.UpdateHealth(-amount);
    }

    public float CalculateDamage(DamageInformation dmgInfo)
    {
        switch (dmgInfo.dmgType) {
            case ScalingTypes.PHYSICAL: {
                    return (dmgInfo.amount * attackDamge) - armor;
                }
            case ScalingTypes.MAGICAL: {
                    return 2;
                }
            default: return 1;
        }
    }

    public void Heal(float amount)
    {
        damageNumber.enableCombination = true;
        damageNumber.SetColor(Color.green);
        damageNumber.Spawn(transform.position, amount);
        health.UpdateHealth(amount);

    }

}
