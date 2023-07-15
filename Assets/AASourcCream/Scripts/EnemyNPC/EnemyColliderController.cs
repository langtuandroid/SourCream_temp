using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderController : MonoBehaviour
{
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
        Debug.Log("DOOOOOOOOOOOOOOIIIIIIIING DAMAGE");

        if (collisionOccured) return;
        if (collider.transform.gameObject.layer == 7) {
            collisionOccured = true;
            var statsComponent = collider.transform.gameObject.GetComponent<StatsComponent>();
            statsComponent.Damage(new DamageInformation(ScalingTypes.PHYSICAL, 20));
        }
    }

    private void OnCollisionExit(Collision collider)
    {
        if (collider.transform.gameObject.layer == 7 && collisionOccured == true) {
            collisionOccured = false;
        }
    }
}
