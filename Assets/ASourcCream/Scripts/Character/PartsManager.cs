using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsManager : MonoBehaviour
{
    [SerializeField]
    private RigData data;

    [SerializeField]
    private List<BodyPart> parts = new List<BodyPart>();

    public void Start() {
        parts.Add(new BodyPart(data.rigHead, data.linksHead));
        parts.Add(new BodyPart(data.rigBody, data.linksBody));
        parts.Add(new BodyPart(data.rigArmLeft, data.linksArmLeft));
        parts.Add(new BodyPart(data.rigArmRight, data.linksArmRight));
        parts.Add(new BodyPart(data.rigLegs, data.linksLegs));
        // TODO - Add wings

        foreach (var part in parts)
        {
            part.SetLinks();
        }
    }

    public void FixedUpdate() {
        foreach (var part in parts)
        {
            part.UpdateLinks();
        }
        // TODO - Update wings
    }
}
