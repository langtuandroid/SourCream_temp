using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsComponent : MonoBehaviour
{
    [SerializeField]
    public Health health = new Health();

    [SerializeField]
    private Dictionary<string, StatModifier> modifiers;

    private float armor = 10.0f;



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
        health.UpdateHealth(-amount);
    }

    public float CalculateDamage(DamageInformation dmgInfo)
    {
        switch (dmgInfo.dmgType) {
            case DamageTypes.Physical: {
                    return dmgInfo.amount - armor;
                }
            case DamageTypes.Magical: {
                    return 2;
                }
            default: return 1;
        }
    }

}
