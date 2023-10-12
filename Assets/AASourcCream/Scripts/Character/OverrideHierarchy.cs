using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OverrideHierarchy
{
    private Transform[] targetRoots;
    private Transform sourceRoot;

    // Bone list used to initialize bones, Looping through boneArray every frame is faster
    // <SourceBone, TargetBone, Offset>
    private Tuple<Transform, Transform>[] boneArray;
    private List<Tuple<Transform, Transform>> boneList;

    public OverrideHierarchy(Transform[] partRoots, Transform joined)
    {
        this.boneList = new List<Tuple<Transform, Transform>>();
        this.boneArray = new Tuple<Transform, Transform>[0];

        this.targetRoots = partRoots;
        this.sourceRoot = joined;

        foreach (Transform target in targetRoots) {
            AddOverrideTransforms(target);
        }

        this.boneArray = this.boneList.ToArray();
    }

    private void AddOverrideTransforms(Transform currentBone)
    {
        var sourceBone = GenericHelper.RecursiveFindChild(sourceRoot, currentBone.name);
        if (sourceBone != null) {
            this.boneList.Add(new Tuple<Transform, Transform>(sourceBone, currentBone));
            sourceBone.position = currentBone.position;
        }

        foreach (Transform childBone in currentBone) {
            AddOverrideTransforms(childBone);
        }
    }

    public void Update()
    {
        foreach (Tuple<Transform, Transform> bones in boneArray) {
            bones.Item2.SetPositionAndRotation(bones.Item1.position, bones.Item1.rotation);
        }
    }
}