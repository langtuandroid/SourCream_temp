using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUI : MonoBehaviour
{
    public class Skill : MonoBehaviour
    {
        public int index;
        public float currentCooldown = 0;
        public float cooldown;
        public string name;

        private Button button;

        public void SetValues(int index, string name, float cooldown, ref Button button)
        {
            this.index = index;
            this.name = name;
            this.cooldown = cooldown;
            this.button = button;
        }

        public void ClickSkill(ClickEvent evt)
        {
            StartCoroutine(CallSkill());
        }

        public IEnumerator CallSkill()
        {
            button.SetEnabled(false);
            currentCooldown = cooldown;
            while (currentCooldown > 0) {
                var valueToTickBy = currentCooldown > 1 ? 1f : 0.1f;
                button.text = currentCooldown > 1 ? Math.Ceiling(currentCooldown).ToString("N0") : currentCooldown.ToString("N1");
                yield return new WaitForSeconds(valueToTickBy); // Wait for 0.1 second
                currentCooldown -= valueToTickBy;
            }

            currentCooldown = 0;
            button.text = this.name;
            button.SetEnabled(true);
        }
    }

    [SerializeField]
    private StatsComponent stats;

    private VisualElement resourcesRoot;
    private VisualElement skillsRoot;
    private List<Skill> skills;

    void Awake()
    {
        // Skills
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        skillsRoot = root.Query<VisualElement>("Skills").First();
        resourcesRoot = root.Query<VisualElement>("Resources").First();
        //GOOD LUCK BUDDY!
        skills = new List<Skill>(new Skill[skillsRoot.childCount]);
    }

    void Update()
    {
        // Resources
        var healthBar = resourcesRoot.Query<ProgressBar>("Health").First();
        healthBar.highValue = stats.health.maxHealth;
        healthBar.value = stats.health.currentHealth;

        var manaBar = resourcesRoot.Query<ProgressBar>("Mana").First();
    }

    public List<Skill> GetSkills()
    {
        return skills;
    }

    public void SetSkill(int index, string name, float cooldown)
    {
        if (index >= 0 && index < skills?.Count) {
            this.UnsetSkill(index);
            Debug.Log(name);
            var skillButton = skillsRoot.Query<Button>().AtIndex(index);
            var skillComponent = this.gameObject.AddComponent<Skill>();
            skillComponent.SetValues(index, name, cooldown, ref skillButton);
            skills[index] = skillComponent;
            skillButton.text = name;

            skillButton.RegisterCallback<ClickEvent>(skills[index].ClickSkill);
        }
    }

    public void UnsetSkill(int index)
    {
        if (index >= 0 && index < skills?.Capacity && skills[index] != null) {
            var skillButton = skillsRoot.Query<Button>().AtIndex(index);
            skillButton.SetEnabled(true);
            skillButton.text = "null";
            skillButton.UnregisterCallback<ClickEvent>(skills[index].ClickSkill);

            // Destroy old Skill component
            var components = this.gameObject.GetComponents<Skill>();
            Destroy(components.Single(c => c.index == index));
        }
    }

    public void CallSkill(int index)
    {
        if (skills[index] != null)
        {
            StartCoroutine(skills[index].CallSkill());
        }
    }

    public void CallSkill(string name)
    {
        var skillToFind = skills.FindIndex(skill => skill.name == name);
        Debug.Log(skillToFind);
        if (skillToFind != -1) {
            StartCoroutine(skills[skillToFind].CallSkill());
        }
    }
}
