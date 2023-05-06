using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

[MessagePackObject]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RarityModifierData", order = 1)]
public class RarityModifierData : ScriptableObject
{
    [Key(0)]
    public string modifierName = "Legendaryyyyy";
    [Key(1)]
    public string colour = "#000000";
    [Key(2)]
    public Dictionary<StatModifierType, Tuple<StatModifierUnit, float>> modifiers;
}