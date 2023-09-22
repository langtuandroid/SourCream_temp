using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Caster
{
    PLAYER,
    ENEMY
}

public class CollisionDetection : MonoBehaviour
{
    public DamageInformation damageInfo { get; set; }

    public StatsComponent statsComponent { get; set; }

    private bool collisionOccured = false;

    public Caster caster = Caster.ENEMY;

    private Action<GameObject> onCollide;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider collider)
    {

        if (collisionOccured) return;
        if (caster == Caster.ENEMY && collider.transform.gameObject.layer == 7) {
            //Debug.Log("HIT PLAYER");
            collisionOccured = true;
            StatsComponent targetStats = collider.gameObject.GetComponent<StatsComponent>();
            targetStats.Damage(damageInfo);
            if (onCollide != null) {
                onCollide(collider.gameObject);
            }
        }
        if (caster == Caster.PLAYER && collider.transform.gameObject.layer == 6) {
            collisionOccured = true;
            StatsComponent targetStats = collider.gameObject.GetComponent<StatsComponent>();
            Debug.Log(targetStats.health.currentHealth);
            targetStats.Damage(damageInfo);
            if (onCollide != null) {
                onCollide(collider.gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collider)
    {
        if (collider.transform.gameObject.layer == 7 && collisionOccured == true) {
            collisionOccured = false;
        }
        if (collider.transform.gameObject.layer == 6 && collisionOccured == true) {
            collisionOccured = false;
        }
    }

    public void SetOnCollide(Action<GameObject> callback = null)
    {
        onCollide = callback;
    }

    public void SetDamageInfo(DamageInformation dmgInfo)
    {
        damageInfo = dmgInfo;
    }
}


