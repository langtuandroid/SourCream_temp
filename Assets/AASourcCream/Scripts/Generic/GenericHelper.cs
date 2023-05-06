using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using MessagePack;

public static class GenericHelper
{
    private static RaycastHit hit;

    //Note will need update for co-op
    public static Vector3 GetMousePostion()
    {
        int layerMask = 1 << 8;
        var hitResult = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask);
        var hitPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        return hitPos;
    }

    /// <summary>
    /// Will try to get a biased number that hasn't been in the previous rolls
    /// </summary>
    public static float GetRandomBiasedNumberBetween(float from, float to, float[] previousRolls)
    {
        float previousResultAverage = previousRolls.Length > 0 ? previousRolls.Average() : 0;
        float randomNumber;

        if (previousResultAverage > 0) {
            if (previousResultAverage > ((from + to) / 2)) {
                randomNumber = Random.Range(from, (to - previousResultAverage));
            } else {
                randomNumber = Random.Range((from + previousResultAverage), to);
            }
        } else {
            randomNumber = Random.Range(from, to);
        }
        return randomNumber;
    }

    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
        Transform result = null;

        foreach (Transform child in parent) {
            if (child.name == childName)
                result = child.transform;
            else
                result = RecursiveFindChild(child, childName);

            if (result != null) break;
        }

        return result;
    }

    public static void WriteBytes<T>(string fileName, T msg)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/SaveData/")) {
            Directory.CreateDirectory(Application.persistentDataPath + "/SaveData/");
        }
        var bytes = MessagePackSerializer.Serialize(msg);
        File.WriteAllBytes(Application.persistentDataPath + "/SaveData/" + fileName, bytes);
    }

    public static T ReadBytes<T>(string fileName)
    {
        var bytes = File.ReadAllBytes(Application.persistentDataPath + "/SaveData/" + fileName);
        return MessagePackSerializer.Deserialize<T>(bytes);
    }
}

public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private List<TKey> keyData = new List<TKey>();

    [SerializeField, HideInInspector]
    private List<TValue> valueData = new List<TValue>();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++) {
            this[this.keyData[i]] = this.valueData[i];
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        this.keyData.Clear();
        this.valueData.Clear();

        foreach (var item in this) {
            this.keyData.Add(item.Key);
            this.valueData.Add(item.Value);
        }
    }
}