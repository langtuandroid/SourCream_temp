using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotLink {
    private Vector3 ctrlPrevFrame;

    [SerializeField]
    private GameObject controller;
    [SerializeField]
    private GameObject puppet;

    public SlotLink(GameObject controller, GameObject puppet)
    {
        this.controller = controller;
        this.puppet = puppet;

        this.ctrlPrevFrame = new Quaternion(
            controller.transform.rotation.x,
            controller.transform.rotation.y,
            controller.transform.rotation.z,
            controller.transform.rotation.w
        ).eulerAngles;
    }

    // Note: Rotation cannot be set directly, as it is relative to other bones
    public void UpdatePuppet()
    {
        // Calculate relative rotation from previous frame
        var currentCtrl = controller.transform.rotation.eulerAngles;
        var relativeRotation = new Vector3(
            currentCtrl.x - this.ctrlPrevFrame.x,
            currentCtrl.y - this.ctrlPrevFrame.y,
            currentCtrl.z - this.ctrlPrevFrame.z
        );
        ctrlPrevFrame = currentCtrl;

        // Apply relative rotation to puppet
        var currentPuppet = puppet.transform.rotation.eulerAngles;
        puppet.transform.rotation = Quaternion.Euler(currentPuppet + relativeRotation);

        // Set puppet world position to controller world position
        puppet.transform.position = controller.transform.position;
    }
}

[System.Serializable]
public class BodyPart
{
    [SerializeField]
    protected List<SlotLink> links;

    protected string rigName;
    protected string[] partLinks;

    public BodyPart(string rigName, string[] partLinks) {
        this.rigName = rigName;
        this.partLinks = partLinks;
    }

    public void UpdateLinks()
    {
        foreach (var link in links)
        {
            link.UpdatePuppet();
        }
    }

    public void SetLinks() {
        links = new List<SlotLink>();

        // Get main rig which will mimic partLinks movements
        var mainRig = GameObject.Find(rigName);
        if(mainRig == null) {
            Debug.LogError("Failed to find main torso rig: " + rigName);
            return;
        }

        foreach (var link in partLinks)
        {
            // Parse link string names
            var names = link.Split(";");
            if(names.Length != 2) {
                Debug.LogError("Invalid torsoLinks input. Expected string format of \"<rig_name>;<bone_name>\"");
                continue;
            };

            // Get other part/bone to link to torso
            var partRig = GameObject.Find(names[0]);
            if(partRig == null) {
                Debug.LogError("Failed to find named rig: " + link);
                continue;
            };

            var linkBone = GenericHelper.RecursiveFindChild(partRig.transform, names[1]);
            if(linkBone == null) {
                Debug.LogError("Failed to find named bone in specified rig: " + link);
                continue;
            }

            // Get torso bone to link
            var mainBone = GenericHelper.RecursiveFindChild(mainRig.transform, names[1]);
            if(mainBone == null) {
                Debug.LogError("Failed to find named torso bone: " + link);
                continue;
            }

            // Link parts
            links.Add(new SlotLink(linkBone.gameObject, mainBone.gameObject));
        }
    }
}