using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

public enum BodySlot
{
    HEAD,
    TORSO,
    ARMS,
    LEGS,
}

[MessagePackObject]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MutationData", order = 1)]
public class MutationData : ScriptableObject
{
    [Key(0)]
    public MutationInfo mutation;
    [Key(1)]
    public SkillInfo skill;
}