using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

public enum WalkStyle
{
    BIPED,
    MULTIPED,
    NA = -1,
}

[MessagePackObject]
[Serializable]
public class MutationInfo
{
    // Technical
    [Key(0)]
    public string mutationName = "mutation name";
    [Key(1)]
    public BodySlot slot;
    [Key(2)]
    public GameObject[] bodyParts;
    [Key(3)]
    public WalkStyle walkStyle = WalkStyle.NA;

    // Gameplay
    [Key(4)]
    public List<MutationData> evolutions;
    [Key(5)]
    public int levelThreshold = 100;
    [Key(6)]
    public bool unlocked = false;

    [Key(7)]
    public RarityModifierData modifier;
}