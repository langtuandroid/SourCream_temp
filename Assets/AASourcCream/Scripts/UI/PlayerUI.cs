using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUI : MonoBehaviour
{
    public class Skill : MonoBehaviour
    {
        public int index;
        public int currentCooldown = 0;
        public int cooldown;
        public string name;

        private Button button;

        public void SetValues(int index, string name, int cooldown, ref Button button)
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
            var callTime = DateTime.Now.AddMilliseconds(cooldown);
            button.SetEnabled(false);
            currentCooldown = cooldown;
            var now = DateTime.Now;
            var isGreate = callTime > now;
            while (callTime > DateTime.Now) {
                currentCooldown -= (int)(Time.deltaTime * 1000);
                button.text = ((float)currentCooldown / 1000).ToString("N2");
                yield return null;
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
    private Skill[] skills;

    void Start()
    {
        // Skills
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        skillsRoot = root.Query<VisualElement>("Skills").First();
        resourcesRoot = root.Query<VisualElement>("Resources").First();
        skills = new Skill[skillsRoot.childCount];
    }

    void Update()
    {
        // Resources
        var healthBar = resourcesRoot.Query<ProgressBar>("Health").First();
        healthBar.highValue = stats.health.maxHealth;
        healthBar.value = stats.health.currentHealth;

        var manaBar = resourcesRoot.Query<ProgressBar>("Mana").First();
    }

    public Skill[] GetSkills()
    {
        return skills;
    }

    public void SetSkill(int index, string name, int cooldown)
    {
        if (index >= 0 && index < skills.Length) {
            this.UnsetSkill(index);

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
        if (index >= 0 && index < skills.Length && skills[index] != null) {
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
        if (skills[index] != null) {
            StartCoroutine(skills[index].CallSkill());
        }
    }
}
