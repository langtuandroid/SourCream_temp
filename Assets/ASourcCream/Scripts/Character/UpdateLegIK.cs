using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpdateLegIK : MonoBehaviour
{
    // IK Controls & Parameters
    public LayerMask terrainLayer;
    public float footSpacing = 0.6f;
    public float stepDistance = 0.8f;
    public float stepHeight = 0.8f;
    public float stepSpeed = 8.0f;

    private float lerp = 0.0f;
    private Vector3 oldPos, currPos, newPos;
    private Vector3 moveDir;

    private Transform charT, target;

    public void Init(Transform character, float lerp)
    {
        charT = character;
        this.lerp = lerp;
    }

    void Start()
    {
        oldPos = currPos = newPos = transform.position;
        moveDir = new Vector3();
    }

    public void UpdateVars(Vector3 inputVelocity)
    {
        moveDir = new Vector3(inputVelocity.x, 0, inputVelocity.z);
    }

    void LateUpdate()
    {
        transform.position = currPos;

        var findNewPos = false;
        var charRay = this.GetCharRay();
        if (Physics.Raycast(charRay, out RaycastHit charInfo, 2f, terrainLayer.value))
        {
            if (Vector3.Distance(oldPos, charInfo.point) > stepDistance)
            {
                findNewPos = true;
            }
        }

        if (findNewPos)
        {
            var nextRay = this.GetNextRay();
            if (Physics.Raycast(nextRay, out RaycastHit nextInfo, 2f, terrainLayer.value))
            {
                lerp = 0;
                oldPos = charInfo.point;
                newPos = nextInfo.point;
            }
            else
            {
                lerp = 0;
                oldPos = charInfo.point;
                newPos = charInfo.point;
            }
        }

        if(lerp < 1) {
            Vector3 tempPosition = Vector3.Lerp(oldPos, newPos, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currPos = tempPosition;
        }
    }

    void Update()
    {
        var templerp = lerp + (Time.deltaTime * stepSpeed);
        lerp = Mathf.Min(templerp, 1.0f);
    }

    Ray GetCharRay()
    {
        var charPos = new Vector3(charT.position.x, charT.position.y+1, charT.position.z);
        var posSpaced = charPos + (charT.right * footSpacing);
        Debug.DrawRay(posSpaced, Vector3.down * 3f, Color.magenta, .001f);
        return new Ray(posSpaced, Vector3.down);
    }
    Ray GetNextRay()
    {
        var charPos = new Vector3(charT.position.x, charT.position.y+1, charT.position.z);
        var nextPos = charPos + (charT.right * footSpacing) + (moveDir * stepDistance);
        Debug.DrawRay(nextPos, Vector3.down * 3f, Color.cyan, .001f);
        return new Ray(nextPos, Vector3.down);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(oldPos, .1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(newPos, .1f);

        Gizmos.color = new Color(.1f, .1f, .1f, .1f);
        Gizmos.DrawSphere(oldPos, stepDistance);
    }
}
