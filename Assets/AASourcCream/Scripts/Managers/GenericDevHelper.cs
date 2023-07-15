using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Sirenix.OdinInspector;
using MessagePack;

public class GenericDevHelper : MonoBehaviour
{
    private static GenericDevHelper _instance;

    public static GenericDevHelper Instance
    {
        get {
            if (_instance is null) {
                Debug.Log("Generic Mono helper is Null");
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    [InfoBox("JSON DUMB SHIT SERIALIZATION WHAT THE FUCK EVEN IS THIS", InfoMessageType.Info)]
    [Button]
    private void UpdateAngle()
    {
        //THIS SUCKS
        //EnemyDataType data = JSONHelper.Read<EnemyDataType>(@"Assets/AASourcCream/Data/JSONData/EnemyNPCs/DummyEnemy.json");
        // byte[] bytes = MessagePackSerializer.Serialize<EnemyDataType>(data);
        // //THIS SUCKS
        // File.WriteAllBytes(@"Assets/AASourcCream/Data/serializedData/EnemyNPCs/DummyEnemy", bytes);
        // Debug.Log(data?.attackDamage);
        // Debug.Log(data.attacks[0]?.value);
    }
}
