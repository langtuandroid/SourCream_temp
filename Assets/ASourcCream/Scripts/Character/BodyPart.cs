using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotLink {
    private Vector3 ctrlPrevFrame;
    public GameObject controller;
    public GameObject puppet;

    public SlotLink Init()
    {
        this.ctrlPrevFrame = new Quaternion(
            controller.transform.rotation.x,
            controller.transform.rotation.y,
            controller.transform.rotation.z,
            controller.transform.rotation.w
        ).eulerAngles;

        return this;
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

public abstract class BodyPart
{
    protected List<SlotLink> links;

    public void UpdateLinks()
    {
        foreach (var link in links)
        {
            link.UpdatePuppet();
        }
    }

    public abstract void SetLinks();
}

[System.Serializable]
public class Torso : BodyPart {
    public SlotLink neck;
    public SlotLink leftArm;
    public SlotLink rightArm;
    public SlotLink leftLeg;
    public SlotLink rightLeg;
    // public SlotLink wings;

    public override void SetLinks() {
        links = new List<SlotLink>();
        links.Add(neck.Init());
        links.Add(leftArm.Init());
        links.Add(rightArm.Init());
        links.Add(leftLeg.Init());
        links.Add(rightLeg.Init());
        // links.Add(wings.Init());
    }
}

[System.Serializable]
public class Head : BodyPart {
    public SlotLink spine;
    public SlotLink leftShoulder;
    public SlotLink rightShoulder;

    public override void SetLinks() {
        links = new List<SlotLink>();
        links.Add(spine.Init());
        links.Add(leftShoulder.Init());
        links.Add(rightShoulder.Init());
    }
}

[System.Serializable]
public class Arms : BodyPart {
    public SlotLink leftShoulder;
    public SlotLink rightShoulder;

    public override void SetLinks() {
        links = new List<SlotLink>();
        links.Add(leftShoulder.Init());
        links.Add(rightShoulder.Init());
    }
}

[System.Serializable]
public class Legs : BodyPart {
    public SlotLink spine;
    public SlotLink hipLeft;
    public SlotLink hipRight;

    public override void SetLinks() {
        links = new List<SlotLink>();
        links.Add(spine.Init());
        links.Add(hipLeft.Init());
        links.Add(hipRight.Init());
    }
}

[System.Serializable]
public class Wings : BodyPart {
    public SlotLink spine;

    public override void SetLinks() {
        links = new List<SlotLink>();
        links.Add(spine.Init());
    }
}
