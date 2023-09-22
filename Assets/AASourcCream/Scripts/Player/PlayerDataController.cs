using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class PlayerDataController : MonoBehaviour
{



    public List<(AbilityDataSerialize serialized, BodyPartAbility ability)> abilityData = new List<(AbilityDataSerialize serialized, BodyPartAbility ability)>();

    [FilePath]
    public string dataLocation = "F:/Code/Unity Projects/SourCream_temp/Assets/AASourcCream/Data/JSONData/Player/BoneBlade.JSON";

    [SerializeField]
    private PlayerUI playerUi;
    private VisualElement root;

    private CooldownManager cooldownManager;

    //NOTE THESE SHOULD BE ORDERED CORRECTLY
    [TableList(ShowPaging = true)]
    public List<AbilityDataSerialize> TableWithPaging = new List<AbilityDataSerialize>()
    {
        new AbilityDataSerialize(),
        new AbilityDataSerialize(),
        new AbilityDataSerialize(),
        new AbilityDataSerialize(),
        new AbilityDataSerialize(),
    };
    // Start is called before the first frame update
    void Start()
    {
        cooldownManager = gameObject.GetComponent<CooldownManager>();
        FakeGetBodyPartArm();
    }

    //TEMP GET DATA THAT IS FAKE, SHOULD ACTUALLY LOAD SOME JSON FROM SOMEWHERE
    public BodyPartItem FakeGetBodyPartArm()
    {
        var data = JSONHelper.Read<BodyPartItem>(TableWithPaging[0].dataLocation);
        //TODO loop like a human
        var ability1 = data.skillTree.abilities[0];
        var ability2 = data.skillTree.abilities[1];
        ability1.onCooldown = false;
        abilityData.Add((TableWithPaging[0], ability1));
        ability2.onCooldown = false;
        abilityData.Add((TableWithPaging[1], ability2));
        SetUpSkills(abilityData);
        return data;
    }
    public (AbilityDataSerialize serialized, BodyPartAbility ability) GetAbilityTuple(int index)
    {
        return abilityData[index];
    }



    public void SetUpSkills(List<(AbilityDataSerialize serialized, BodyPartAbility ability)> abilities)
    {
        for (int i = 0; i < abilities.Count; i++) {
            playerUi.SetSkill(i, abilities[i].ability.name, abilities[i].ability.cooldown);
        }
    }
}

[Serializable]
public class AbilityDataSerialize
{
    [TableColumnWidth(70, Resizable = false)]
    [PreviewField(Alignment = ObjectFieldAlignment.Center)]
    public Texture abilityIcon;
    [FilePath]
    public string dataLocation = "F:/Code/Unity Projects/SourCream_temp/Assets/AASourcCream/Data/JSONData/Player/BoneBlade.JSON";

    [VerticalGroup("Names"), LabelWidth(80)]
    public string ability, bodyPart;
    [VerticalGroup("Prefabs"), LabelWidth(80)]
    public GameObject Indicator, Collider, VFX, SFX;

    [SerializeField]
    public IAbilityImplementation abilityImplementation;

    [OnInspectorInit]
    private void CreateData()
    {
        Debug.Log(abilityImplementation);
        abilityIcon = null;
    }
}