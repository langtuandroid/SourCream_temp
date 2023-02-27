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
        // Wing link/bone/rig
    };

    // Head
    public string rigHead = "rig_head";
    public string[] linksHead = {
        "rig_body;spine.003",       // Neck
        "rig_body;shoulder.L",      // Left Shoulder
        "rig_body;shoulder.R",      // Right Shoulder
    };

    // Arms
    public string rigArmLeft = "rig_arm_L";
    public string[] linksArmLeft = {
        "rig_body;shoulder.L",      // Left Shoulder
    };

    public string rigArmRight = "rig_arm_R";
    public string[] linksArmRight = {
        "rig_body;shoulder.R",      // Right Shoulder
    };

    // Legs
    public string rigLegs = "rig_legs";
    public string[] linksLegs = {
        "rig_body;spine",           // Lower back
        "rig_body;pelvis.L",        // Left hip
        "rig_body;pelvis.R",        // Right hip
    };

    // Wings

    /*** Rig target links ***/
    public string head = "head_target";
    public string body = "spine.003";
    public string armRight = "arm_target.R";
    public string armLeft = "arm_target.L";
    public string hip = "spine";
    public string legRight = "leg_target.R";
    public string legLeft = "leg_target.L";

    // Target Position constraints

    // Target Rotation constraints

}
