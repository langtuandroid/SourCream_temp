using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using FIMSpace.FProceduralAnimation;

public class PartsManager : MonoBehaviour
{
    [SerializeField]
    private RigData data;

    [SerializeField]
    private PlayerMutations equippedMutations;

    [SerializeField]
    private GameObject mutationPartsRoot;
    [SerializeField]
    private GameObject joinedRig;
    [SerializeField]
    private GameObject ikConstraintsRoot;

    private OverrideHierarchy overrideHierarchy;

    public void Awake()
    {
        RebuildParts();
    }

    public void LateUpdate()
    {
        this.overrideHierarchy?.Update();
    }

    public void RebuildParts()
    {
        // Disable animator while assigning/changing rigs and IK constraints
        var animator = GetComponent<Animator>();
        animator.enabled = false;

        // Spawn body parts under root "Parts" object
        var height = BuildBody();

        // Set "Parts" object height
        // This is to account for the general origin being in the centre of the torso,
        // while we want bottom of all parts combined to be the soles of the feet
        var localPos = mutationPartsRoot.transform.localPosition;
        localPos.y = height;
        mutationPartsRoot.transform.localPosition = localPos;

        // Build a single, joined, skeleton that can easily control all parts together
        BuildJoined();

        // Assign IK tips & roots
        foreach (var tipAndRoot in data.tipsAndRoots) {
            SpawnIkConstraint(tipAndRoot.Key, tipAndRoot.Value.tip, tipAndRoot.Value.root);
        }

        // Setup LegsAnimator component
        AttachLegsAnimator();

        // Rebuild rig and Rebind animator
        GetComponent<RigBuilder>().Build();
        animator.enabled = true;
    }

    private float BuildBody()
    {
        // Destroy old body parts if exist
        var tmpParts = mutationPartsRoot.transform.Cast<Transform>().ToList();
        foreach (Transform part in tmpParts) {
            part.parent = null;
            GameObject.Destroy(part.gameObject);
        }

        // Destroy old constraints if exist
        var tmpConst = ikConstraintsRoot.transform.Cast<Transform>().ToList();
        foreach (Transform constraint in tmpConst) {
            constraint.parent = null;
            GameObject.Destroy(constraint.gameObject);
        }

        float originHeight = 0.0f;
        // Instantiate new body parts
        var muts = equippedMutations.mutations;
        if (muts.Keys.Count > 0) {
            var playerHeight = GetComponent<CharacterController>().center.y;
            // Torso
            var torso = GameObject.Instantiate(muts[BodySlot.TORSO].mutation.bodyParts[0], mutationPartsRoot.transform);
            torso.name = BodySlot.TORSO.ToString();

            // Following parts need to calculate their position based on positional slots on the torso prefab
            // Head
            var headSlotPosition = torso.transform.localPosition + torso.transform.Find(BodySlot.HEAD.ToString()).localPosition;
            var head = GameObject.Instantiate(muts[BodySlot.HEAD].mutation.bodyParts[0], mutationPartsRoot.transform);
            head.transform.localPosition = headSlotPosition;
            head.name = BodySlot.HEAD.ToString();

            // Arms
            var armleftSlotPosition = torso.transform.localPosition + torso.transform.Find(BodySlot.ARMS.ToString() + ".L").localPosition;
            var armLeft = GameObject.Instantiate(muts[BodySlot.ARMS].mutation.bodyParts[0], mutationPartsRoot.transform);
            armLeft.transform.localPosition = armleftSlotPosition;
            armLeft.name = BodySlot.ARMS.ToString() + ".L";
            var armRightSlotPosition = torso.transform.localPosition + torso.transform.Find(BodySlot.ARMS.ToString() + ".R").localPosition;
            var armRight = GameObject.Instantiate(muts[BodySlot.ARMS].mutation.bodyParts[1], mutationPartsRoot.transform);
            armRight.transform.localPosition = armRightSlotPosition;
            armRight.name = BodySlot.ARMS.ToString() + ".R";

            // Legs
            var legleftSlotPosition = torso.transform.localPosition + torso.transform.Find(BodySlot.LEGS.ToString() + ".L").localPosition;
            var legLeft = GameObject.Instantiate(muts[BodySlot.LEGS].mutation.bodyParts[0], mutationPartsRoot.transform);
            legLeft.transform.localPosition = legleftSlotPosition;
            legLeft.name = BodySlot.LEGS.ToString() + ".L";
            var legRightSlotPosition = torso.transform.localPosition + torso.transform.Find(BodySlot.LEGS.ToString() + ".R").localPosition;
            var legRight = GameObject.Instantiate(muts[BodySlot.LEGS].mutation.bodyParts[1], mutationPartsRoot.transform);
            legRight.transform.localPosition = legRightSlotPosition;
            legRight.name = BodySlot.LEGS.ToString() + ".R";

            // Calculate origin height = distance between leg.L.ground.y and torso.y
            var ground = legLeft.transform.Find("ground");
            if (ground == null) {
                Debug.LogError("Left leg requires a 'ground' object, to calculate the player's height");
            } else {
                originHeight = torso.transform.position.y - ground.position.y;
            }
        } else {
            Debug.LogError("Player PartsManager.EquippedMutations.Keys must contain BodySlots HEAD, TORSO, ARMS, and LEGS");
        }

        return originHeight;
    }

    private void BuildJoined()
    {
        // Destroy old joined rig
        var tmpRig = joinedRig.transform.Cast<Transform>().ToList();
        foreach (Transform rig in tmpRig) {
            rig.parent = null;
            GameObject.Destroy(rig.gameObject);
        }

        // Create new rig, with torso rig as base
        var torsoRig = mutationPartsRoot.transform.Find(BodySlot.TORSO.ToString())
            .GetChild(0); // E.g. Parts -> TORSO -> rig_body
        var newRig = GameObject.Instantiate(torsoRig);
        newRig.name = "full.Rig";
        newRig.parent = joinedRig.transform;
        newRig.transform.position = torsoRig.position;

        // Duplicate and attach other part rigs to torso, and replace any duplicate bones
        foreach (var link in data.links) {
            ReplacePartRigs(newRig, link.Key, link.Value);
        }

        // Override transforms of all bones in each part rig with the respective bones in the joined rig
        Transform[] partTransforms = {
            torsoRig,
            GenericHelper.RecursiveFindChild(
                mutationPartsRoot.transform.Find(BodySlot.HEAD.ToString()),
                data.links[BodySlot.HEAD.ToString()]),
            GenericHelper.RecursiveFindChild(
                mutationPartsRoot.transform.Find(BodySlot.ARMS.ToString() + ".L"),
                data.links[BodySlot.ARMS.ToString() + ".L"]),
            GenericHelper.RecursiveFindChild(
                mutationPartsRoot.transform.Find(BodySlot.ARMS.ToString() + ".R"),
                data.links[BodySlot.ARMS.ToString() + ".R"]),
            GenericHelper.RecursiveFindChild(
                mutationPartsRoot.transform.Find(BodySlot.LEGS.ToString() + ".L"),
                data.links[BodySlot.LEGS.ToString() + ".L"]),
            GenericHelper.RecursiveFindChild(
                mutationPartsRoot.transform.Find(BodySlot.LEGS.ToString() + ".R"),
                data.links[BodySlot.LEGS.ToString() + ".R"]),
        };
        this.overrideHierarchy = new OverrideHierarchy(partTransforms, this.joinedRig.transform);
    }

    private void ReplacePartRigs(Transform newRig, string slot, string bone)
    {
        var partRoot = mutationPartsRoot.transform.Find(slot);
        var partRig = GenericHelper.RecursiveFindChild(partRoot, bone);

        var torsoSlot = GenericHelper.RecursiveFindChild(newRig, bone);

        var tmpSlotTransform = torsoSlot.transform.position;
        var tmpSlotParent = torsoSlot.parent;

        var partRigClone = GameObject.Instantiate(partRig, tmpSlotTransform, partRig.rotation);
        partRigClone.parent = tmpSlotParent;
        partRigClone.name = partRig.name;

        torsoSlot.parent = null;
        GameObject.Destroy(torsoSlot.gameObject);
    }

    private void AddOverrideTransforms(Transform parentBone)
    {
        foreach (Transform bone in parentBone) {
            var overrideBone = GenericHelper.RecursiveFindChild(joinedRig.transform, bone.name);
            if (overrideBone == null) continue;

            var overrideComponent = bone.gameObject.AddComponent<OverrideTransform>();
            overrideComponent.Reset();

            overrideComponent.data.constrainedObject = bone;
            overrideComponent.data.sourceObject = overrideBone;

            // Loop through entire skeleton, until all bones are marked
            AddOverrideTransforms(bone);
        }
    }

    private void SpawnIkConstraint(string slot, string tip, string root)
    {
        var tipTrans = GenericHelper.RecursiveFindChild(joinedRig.transform, tip);
        var rootTrans = GenericHelper.RecursiveFindChild(joinedRig.transform, root);

        var constraint = new GameObject(slot);
        constraint.transform.SetParent(ikConstraintsRoot.transform);
        constraint.transform.localPosition = new Vector3(0, 0, 0);

        var target = new GameObject("target_" + slot);
        target.transform.SetParent(constraint.transform);

        var chain = constraint.AddComponent<ChainIKConstraint>();
        chain.Reset(); // Set default values

        chain.data.tip = tipTrans;
        chain.data.root = rootTrans;
        chain.data.target = target.transform;

        target.transform.SetPositionAndRotation(tipTrans.position, tipTrans.rotation);
    }

    private void AttachLegsAnimator()
    {
        LegsAnimator legsAnimator = this.gameObject.AddComponent<LegsAnimator>();
        legsAnimator.MotionInfluence = new MotionInfluenceProcessor(); // Will not be required to call in next update
        legsAnimator.HipsSetup = new LegsAnimator.HipsReference(); // Will not be required to call in next update
        legsAnimator.Event_OnStep = new UnityEngine.Events.UnityEvent(); // Will not be required to call in next update
        legsAnimator.BaseLegAnimating = new LegsAnimator.LegStepAnimatingParameters(); // Will not be required to call in next update

        legsAnimator.Hips = this.joinedRig.transform.GetChild(0).GetChild(0);

        // Initialize legs
        LegsAnimator.LegStepAnimatingParameters stepParams = new();
        stepParams.RefreshDefaultCurves();
        LegMotionSettingsPreset legSettings = (LegMotionSettingsPreset)ScriptableObject.CreateInstance("LegMotionSettingsPreset");
        legSettings.Settings = stepParams;

        Transform leftLegStart = GenericHelper.RecursiveFindChild(joinedRig.transform, "thigh.L");
        LegsAnimator.Leg leftLeg = new() {
            BoneStart = leftLegStart,
            BoneMid = leftLegStart.GetChild(0),
            BoneEnd = leftLegStart.GetChild(0).GetChild(0),
            Side = LegsAnimator.ELegSide.Left,
            OppositeLegIndex = 1,
            CustomLegAnimating = legSettings,
            Owner = legsAnimator,
        };
        Transform rightlegStart = GenericHelper.RecursiveFindChild(joinedRig.transform, "thigh.R");
        LegsAnimator.Leg rightLeg = new() {
            BoneStart = rightlegStart,
            BoneMid = rightlegStart.GetChild(0),
            BoneEnd = rightlegStart.GetChild(0).GetChild(0),
            Side = LegsAnimator.ELegSide.Right,
            OppositeLegIndex = 0,
            CustomLegAnimating = legSettings,
            Owner = legsAnimator,
        };
        legsAnimator.Legs = new List<LegsAnimator.Leg> { leftLeg, rightLeg };

        leftLeg.InitLegBasics(legsAnimator, 0, rightLeg);
        rightLeg.InitLegBasics(legsAnimator, 1, null);

        legsAnimator.Finders_RefreshAllLegsAnkleAxes();
        legsAnimator.BaseLegAnimating.RefreshDefaultCurves();

        // Set terrain layer
        LayerMask terrain = LayerMask.GetMask("Terrain");
        legsAnimator.GroundMask = terrain;
    }

    private void SaveData()
    {
        GenericHelper.WriteBytes("equipped", equippedMutations);
    }

    private void LoadData()
    {
        try {
            this.equippedMutations = GenericHelper.ReadBytes<PlayerMutations>("equipped");
        } catch (System.Exception) {
            throw;
        }
    }
}
