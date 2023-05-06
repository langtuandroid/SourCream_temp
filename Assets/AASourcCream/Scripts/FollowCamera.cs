using Sirenix.OdinInspector;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    Transform target;

    [SerializeField]
    float distanceY;
    [SerializeField]
    float distanceZ;
    [InfoBox("Offsets the camera x rotation thus making your character not in the middle(higher offset = camera has more space above)", InfoMessageType.Info)]
    public float rotationXoffset;

    [InfoBox("Recalculates the x rotation for camera based on Distance Y, Distance Z and RottationXoffset", InfoMessageType.Info)]
    [Button]
    private void UpdateAngle()
    {
        mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
        UpdateCameraAngle();
    }


    // Start is called before the first frame update
    void Start()
    {
        var newPos = new Vector3(x: target.position.x, y: target.position.y + distanceY, z: target.position.z - distanceZ);
        mainCamera.transform.position = newPos;
        UpdateCameraAngle();
    }


    // Update is called once per frame
    void LateUpdate()
    {
        var screenPos = Camera.main.WorldToScreenPoint(target.position);
        var ray = Camera.main.ScreenPointToRay(screenPos);
        var ray2 = new Vector3(ray.direction.x, ray.direction.y - 20, ray.direction.z);
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);
        //TODO: make movement delayed
        mainCamera.transform.position = new Vector3(x: target.position.x, y: target.position.y + distanceY, z: target.position.z - distanceZ);
    }

    void UpdateCameraAngle()
    {
        var screenPos = Camera.main.WorldToScreenPoint(target.position);
        var ray = Camera.main.ScreenPointToRay(screenPos);
        var ray2 = new Vector3(ray.direction.x, ray.direction.y - 20, ray.direction.z);
        var angle = Vector3.Angle(ray.direction, ray2);
        mainCamera.transform.rotation = Quaternion.Euler(90.0f - angle - rotationXoffset, 0, 0);
    }
}