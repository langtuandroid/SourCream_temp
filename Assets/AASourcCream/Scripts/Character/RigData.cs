using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RigData", order = 1)]
public class RigData : ScriptableObject
{
    // Generic types cannot be serialized by Unity Editor directly
    [Serializable]
    public class DictBodyLinks : UnitySerializedDictionary<string, string> { }
    [Serializable]
    public struct TupleBones
    {
        public TupleBones(string tip, string root)
        {
            this.tip = tip;
            this.root = root;
        }
        public string tip; public string root;
    }
    [Serializable]
    public class DictIKSets : UnitySerializedDictionary<string, TupleBones> { }

    /*** Bone placements & rig links to Torso ***/
    public DictBodyLinks links = new DictBodyLinks {
        { BodySlot.HEAD.ToString(), "spine.004" },
        { BodySlot.ARMS.ToString() + ".L", "upper_arm.L" },
        { BodySlot.ARMS.ToString() + ".R", "upper_arm.R" },
        { BodySlot.LEGS.ToString() + ".L", "thigh.L" },
        { BodySlot.LEGS.ToString() + ".R", "thigh.R" },
    };

    /*** IK Tips ***/
    public DictIKSets tipsAndRoots = new DictIKSets {
        { BodySlot.HEAD.ToString(), new TupleBones("spine.006", "spine.004") },
        { BodySlot.ARMS.ToString() + ".L", new TupleBones("hand.L", "upper_arm.L") },
        { BodySlot.ARMS.ToString() + ".R", new TupleBones("hand.R", "upper_arm.R") },
    };

    // TODO - determine if this is needed anywhere/anymore
    // Idea is to use these as core bones for animating whole rig
    /*** Rig target links ***/
    public string head = "head_target";
    public string body = "spine.003";
    public string armRight = "arm_target.R";
    public string armLeft = "arm_target.L";
    public string hip = "spine";
    public string legRight = "leg_target.R";
    public string legLeft = "leg_target.L";
}
