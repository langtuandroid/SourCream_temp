using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

public enum ColliderTypes
{
    SimpleSquare,
    SimpleCone,
    SimpleCircle,
    SimpleRectangle,
    SimpleSphere,
    SimpleCapsule
}

public class GenericColliderHelper : SerializedMonoBehaviour
{
    [FoldoutGroup("Colliders", expanded: false)]
    public Dictionary<ColliderTypes, GameObject> colliders;
    private static GenericColliderHelper _instance;
    public static GenericColliderHelper Instance
    {
        get {
            if (_instance is null) {
                Debug.Log("Generic collider helper is Null");
            }
            return _instance;
        }
    }

    void Start()
    {

    }

    private void Awake()
    {
        _instance = this;
    }

    public void EnemySpawnCollider(ColliderTypes colliderType, Transform location, float duration, DamageInformation dmgInfo)
    {

        GameObject collider = null;
        switch (colliderType) {
            case ColliderTypes.SimpleSquare:
                collider = ParamInstantiate.InstantiateColliderWithInfo(colliders[ColliderTypes.SimpleSquare], location.position, location.rotation, location.transform, dmgInfo);
                break;
            case ColliderTypes.SimpleCone:
                collider = ParamInstantiate.InstantiateColliderWithInfo(colliders[ColliderTypes.SimpleCircle], location.position, location.rotation, location.transform, dmgInfo);
                break;

            case ColliderTypes.SimpleCircle:
                collider = ParamInstantiate.InstantiateColliderWithInfo(colliders[ColliderTypes.SimpleSphere], location.position, location.rotation, location.transform, dmgInfo);
                break;

            case ColliderTypes.SimpleRectangle:
                collider = ParamInstantiate.InstantiateColliderWithInfo(colliders[ColliderTypes.SimpleCapsule], location.position, location.rotation, location.transform, dmgInfo);
                break;
            case ColliderTypes.SimpleCapsule:
                collider = ParamInstantiate.InstantiateColliderWithInfo(colliders[ColliderTypes.SimpleCapsule], location.position, location.rotation, location.transform, dmgInfo);
                break;
        }

        if (collider != null) {
            StartCoroutine(GenericMonoHelper.Instance.DestroyAfterDelay(duration, collider));
        }
    }

}

public static class ParamInstantiate
{
    /// <summary>
    /// Calls Instantiate gets the collision script from it and tries to attach the provided dmgInfo to it if it doesn't find a collision script will return null
    /// </summary>
    public static GameObject InstantiateColliderWithInfo(GameObject original, Vector3 position, Quaternion rotation, Transform parent, DamageInformation dmgInfo)
    {
        var returnValue = UnityEngine.Object.Instantiate(original, position, rotation, parent);
        CollisionDetection collision = returnValue.GetComponent<CollisionDetection>();
        if (collision) {
            collision.SetDamageInfo(dmgInfo);
            return returnValue;
        }

        return null;
    }
};