using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RigData", order = 1)]
public class RigData : ScriptableObject
{
    /*** Bone placements & links ***/
    // rig variable determines who is the puppet
    // links variable determines which rig controls which bones

    // Body
    public string rigBody = "rig_body";
    public string[] linksBody = {
        "rig_head;spine.004",       // Neck
        "rig_arm_L;upper_arm.L",    // Left Arm
        "rig_arm_R;upper_arm.R",    // Right Arm
        "rig_legs;thigh.L",         // Left Leg
        "rig_legs;thigh.R"          // Right Leg
    };

    /*** Rig target links ***/
    public string head = "head_target";
    public string body = "spine.003";
    public string armRight = "arm_target.R";
    public string armLeft = "arm_target.L";
    public string hip = "spine";
    public string legRight = "leg_target.R";
    public string legLeft = "leg_target.L";

    /*** IK Tips ***/
    public Dictionary<BodySlot, Tuple<string, string>> tipsAndRoots = new Dictionary<BodySlot, Tuple<string, string>> {
        { BodySlot.HEAD, Tuple.Create("face", "spine.004") },
        { BodySlot.ARMS, Tuple.Create("hand.X", "upper_arm.X") },
        { BodySlot.LEGS, Tuple.Create("foot.X", "thigh.X") },
    };
}
