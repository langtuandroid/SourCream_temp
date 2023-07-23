using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugUI : MonoBehaviour
{
    [SerializeField]
    private PlayerUI playerUi;
    private VisualElement root;

    void Start()
    {
        // Skills
        root = GetComponent<UIDocument>().rootVisualElement;
        var setSkill = root.Query<Button>("SetSkillOne").First();
        var unsetSkill = root.Query<Button>("UnsetSkillOne").First();

        setSkill.RegisterCallback<ClickEvent>(SetSkill);
        unsetSkill.RegisterCallback<ClickEvent>(UnsetSkill);
    }

    void SetSkill(ClickEvent evt)
    {
        var cooldown = root.Query<IntegerField>("Cooldown").First();
        var name = root.Query<TextField>("Name").First();
        playerUi.SetSkill(0, name.value, cooldown.value);
    }

    void UnsetSkill(ClickEvent evt)
    {
        playerUi.UnsetSkill(0);
    }
}
