using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public DamageInformation damageInfo { get; private set; }

    private bool collisionOccured = false;
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
        if (collider.transform.gameObject.layer == 7)
        {
            collisionOccured = true;
            var statsComponent = collider.transform.gameObject.GetComponent<StatsComponent>();
            statsComponent.Damage(damageInfo);
        }
    }

    private void OnCollisionExit(Collision collider)
    {
        if (collider.transform.gameObject.layer == 7 && collisionOccured == true)
        {
            collisionOccured = false;
        }
    }

    public void SetDamageInfo(DamageInformation dmgInfo)
    {
        damageInfo = dmgInfo;
    }
}


