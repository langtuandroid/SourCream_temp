using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PartsManager : MonoBehaviour
{
    [SerializeField]
    private RigData data;

    [SerializeField]
    private List<BodyPart> parts = new List<BodyPart>();

    [SerializeField]
    private PlayerMutations equippedMutations;
    private Dictionary<BodySlot, GameObject> mutationParts;

    [SerializeField]
    private GameObject mutationPartsRoot;
    [SerializeField]
    private GameObject ikConstraintsRoot;

    public void Awake()
    {
        // Disable animator while assigning/changing rigs and IK constraints
        var animator = GetComponent<Animator>();
        animator.enabled = false;

        // Spawn body parts under root "Parts" object
        BuildBody();

        // Assign IK tips & roots
        foreach (var tipAndRoot in data.tipsAndRoots) {
            SetupChainIK(tipAndRoot.Key, tipAndRoot.Value.Item1, tipAndRoot.Value.Item2);
        }

        // Assign walking style w/ targets - TODO - do crab walk, etc. Currently only biped implemented
        if (equippedMutations.mutations.ContainsKey(BodySlot.LEGS)) {
            if (equippedMutations.mutations[BodySlot.LEGS].mutation.walkStyle == WalkStyle.BIPED) {
                var biped = GetComponent<BipedIK>();
                biped.enabled = true;

                var targetL = GenericHelper.RecursiveFindChild(ikConstraintsRoot.transform, "target_LEGS.L");
                var targetR = GenericHelper.RecursiveFindChild(ikConstraintsRoot.transform, "target_LEGS.R");
                biped.leftFootTargetRig = targetL;
                biped.rightFootTargetRig = targetR;
            }
        } else {
            Debug.LogError("No leg mutation equipped. Cannot set IK Targets.");
        }

        // Rebuild rig and Rebind animator
        GetComponent<RigBuilder>().Build();
        animator.enabled = true;

        // Attach body part links
        parts.Add(new BodyPart(data.rigBody, data.linksBody));

        foreach (var part in parts) {
            part.SetLinks();
        }
    }

    public void FixedUpdate()
    {
        foreach (var part in parts) {
            part.UpdateLinks();
        }
    }

    private void BuildBody()
    {
        // Destroy old body parts if exist
        var tmpParts = mutationPartsRoot.transform.Cast<Transform>().ToList();
        foreach (Transform part in tmpParts) {
            part.parent = null;
            GameObject.Destroy(part.gameObject);
        }
        this.parts.Clear();

        // Destroy old constraints if exist
        var tmpConst = ikConstraintsRoot.transform.Cast<Transform>().ToList();
        foreach (Transform constraint in tmpConst) {
            constraint.parent = null;
            GameObject.Destroy(constraint.gameObject);
        }

        // Instantiate new body parts
        var muts = equippedMutations.mutations;
        if (muts.Keys.Count > 0) {
            var head = new GameObject(BodySlot.HEAD.ToString());
            head.transform.parent = mutationPartsRoot.transform;
            GameObject.Instantiate(muts[BodySlot.HEAD].mutation.bodyParts[0], head.transform);

            var torso = new GameObject(BodySlot.TORSO.ToString());
            torso.transform.parent = mutationPartsRoot.transform;
            GameObject.Instantiate(muts[BodySlot.TORSO].mutation.bodyParts[0], torso.transform);

            var arms = new GameObject(BodySlot.ARMS.ToString());
            arms.transform.parent = mutationPartsRoot.transform;
            GameObject.Instantiate(muts[BodySlot.ARMS].mutation.bodyParts[0], arms.transform);
            arms.transform.parent = mutationPartsRoot.transform;
            GameObject.Instantiate(muts[BodySlot.ARMS].mutation.bodyParts[1], arms.transform);

            var legs = new GameObject(BodySlot.LEGS.ToString());
            legs.transform.parent = mutationPartsRoot.transform;
            GameObject.Instantiate(muts[BodySlot.LEGS].mutation.bodyParts[0], legs.transform);
        } else {
            Debug.LogError("Player PartsManager.EquippedMutations.Keys must contain BodySlots HEAD, TORSO, ARMS, and LEGS");
        }
    }

    private void SetupChainIK(BodySlot slot, string tip, string root)
    {
        // Assume X = polarity (e.g. hand.X == hand.L)
        if (tip.Contains("X") || root.Contains("X")) {
            SpawnIkConstraint(slot.ToString(), ".L", tip.Replace("X", "L"), root.Replace("X", "L"));
            SpawnIkConstraint(slot.ToString(), ".R", tip.Replace("X", "R"), root.Replace("X", "R"));
        } else {
            SpawnIkConstraint(slot.ToString(), "", tip, root);
        }
    }

    private void SpawnIkConstraint(string slot, string suffix, string tip, string root)
    {
        var mutationPart = mutationPartsRoot.transform.Find(slot);
        var tipTrans = GenericHelper.RecursiveFindChild(mutationPart, tip);
        var rootTrans = GenericHelper.RecursiveFindChild(mutationPart, root);

        var constraint = new GameObject(slot + suffix);
        constraint.transform.SetParent(ikConstraintsRoot.transform);
        constraint.transform.localPosition = new Vector3(0, 0, 0);

        var target = new GameObject("target_" + slot + suffix);
        target.transform.SetParent(constraint.transform);

        var chain = constraint.AddComponent<ChainIKConstraint>();
        chain.Reset(); // Set default values

        chain.data.tip = tipTrans;
        chain.data.root = rootTrans;
        chain.data.target = target.transform;

        target.transform.position = tipTrans.position;
        target.transform.rotation = tipTrans.rotation;
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
