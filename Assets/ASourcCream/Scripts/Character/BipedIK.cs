using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipedIK : MonoBehaviour
{

    /* Some useful functions we may need */

    static Vector3[] CastOnSurface(Vector3 point, float halfRange, Vector3 up, LayerMask layer)
    {
        Vector3[] res = new Vector3[2];
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(point.x, point.y + halfRange, point.z), -up);

        if (Physics.Raycast(ray, out hit, 2f * halfRange, layer.value))
        {
            res[0] = hit.point;
            // res[1] = hit.normal;
        }
        else
        {
            res[0] = point;
        }
        return res;
    }

    /*************************************/


    public LayerMask terrain;
    public Transform leftFootTarget;
    public Transform rightFootTarget;
    public Transform leftFootTargetRig;
    public Transform rightFootTargetRig;
    public Transform pivot;
    public Transform scaler;
    
    public float smoothness = 2f;
    public float stepHeight = 0.2f;
    public float stepLength = 1f;
    public float angularSpeed = 0.1f;
    public float velocityMultiplier = 80f;

    private Vector3 lastBodyPos;

    private Vector3 velocity;
    private Vector3 lastVelocity;

    // Start is called before the first frame update
    void Start()
    {
        lastBodyPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Calculate player velocity smoothly
        velocity = transform.position - lastBodyPos;
        velocity *= velocityMultiplier;
        velocity = (velocity + smoothness * lastVelocity) / (smoothness + 1f);

        if (velocity.magnitude < 0.00025f * velocityMultiplier)
            velocity = lastVelocity;
        lastVelocity = velocity;

        scaler.localScale = new Vector3(scaler.localScale.x, stepHeight * 2f * velocity.magnitude, stepLength * velocity.magnitude);
        
        int sign = (Vector3.Dot(velocity.normalized, transform.forward) < 0 ? -1 : 1);
        // Rotate root scaler in direction of movement (sign) (around y axis)
        scaler.LookAt(transform.position + (velocity * sign));
        // Spin pivot in direction of movement (sign) (around x axis)
        pivot.Rotate(Vector3.right, sign * angularSpeed, Space.Self);

        Vector3 desiredPositionLeft = leftFootTarget.position;
        Vector3 desiredPositionRight = rightFootTarget.position;

        // Set to raycast hit if y > spin y position
        // Update left leg
        Vector3[] posNormLeft = CastOnSurface(desiredPositionLeft, 2f, Vector3.up, terrain);
        if (posNormLeft[0].y > desiredPositionLeft.y)
        {
            leftFootTargetRig.position = posNormLeft[0];
        }
        else
        {
            leftFootTargetRig.position = desiredPositionLeft;
        }
        if (posNormLeft[1] != Vector3.zero)
        {
            leftFootTargetRig.rotation = Quaternion.LookRotation(sign * velocity.normalized, posNormLeft[1]);
        }

        // Update right leg
        Vector3[] posNormRight = CastOnSurface(desiredPositionRight, 2f, Vector3.up, terrain);
        if (posNormRight[0].y > desiredPositionRight.y)
        {
            rightFootTargetRig.position = posNormRight[0];
        }
        else
        {
            rightFootTargetRig.position = desiredPositionRight;
        }
        if(posNormRight[1] != Vector3.zero)
        {
            rightFootTargetRig.rotation = Quaternion.LookRotation(sign * velocity.normalized, posNormRight[1]);
        }

        lastBodyPos = transform.position;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(leftFootTarget.position, 0.2f);
        Gizmos.DrawWireSphere(rightFootTarget.position, 0.2f);
    }
}