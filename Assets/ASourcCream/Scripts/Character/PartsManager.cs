using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsManager : MonoBehaviour
{
    public Torso torso;
    public Head head;
    public Arms arms;
    public Legs legs;
    // public Wings wings;

    public void Start() {
        torso.SetLinks();
        head.SetLinks();
        arms.SetLinks();
        legs.SetLinks();
        // wings.SetLinks();
    }

    public void FixedUpdate() {
        torso.UpdateLinks();
        head.UpdateLinks();
        arms.UpdateLinks();
        legs.UpdateLinks();
        // wings.UpdateLinks();
    }
}
