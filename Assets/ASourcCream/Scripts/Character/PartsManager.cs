using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsManager : MonoBehaviour
{
    [SerializeField]
    private RigData data;

    [SerializeField]
    private List<BodyPart> parts = new List<BodyPart>();

    public void Awake()
    {
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
}
