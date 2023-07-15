using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyDataController : MonoBehaviour
{
    [FilePath]
    public string dataLocation = "F:/Code/Unity Projects/SourCream_temp/Assets/AASourcCream/Data/JSONData/EnemyNPCs/DummyEnemy";

    public EnemyDataType enemyData { get; private set; }

    void Awake()
    {
        enemyData = JSONHelper.Read<EnemyDataType>(dataLocation);
        Debug.Log(enemyData.name);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
