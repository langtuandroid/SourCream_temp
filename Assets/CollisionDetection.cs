using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {

        if (collisionOccured) return;
        if (caster == Caster.ENEMY && collider.transform.gameObject.layer == 7) {
            //Debug.Log("HIT PLAYER");
            collisionOccured = true;
            StatsComponent targetStats = collider.gameObject.GetComponent<StatsComponent>();
            targetStats.Damage(damageInfo);
        }
        if (caster == Caster.PLAYER && collider.transform.gameObject.layer == 6) {
            collisionOccured = true;
            StatsComponent targetStats = collider.gameObject.GetComponent<StatsComponent>();
            Debug.Log(targetStats.health.currentHealth);
            targetStats.Damage(damageInfo);
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

    public void SetDamageInfo(DamageInformation dmgInfo)
    {
        damageInfo = dmgInfo;
    }
}


