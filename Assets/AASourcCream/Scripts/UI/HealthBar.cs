using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private GameObject character;

    private ProgressBar healthbar;

    private StatsComponent characterStats;
    // Start is called before the first frame update
    void Start()
    {
        characterStats = character.GetComponent<StatsComponent>();
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        healthbar = root.Query<ProgressBar>("HealthBar");
        healthbar.highValue = characterStats.health.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {

    }

    private void FixedUpdate()
    {
        //IDK MAYBE DO EVENTS INSTEAD THIS SEEMS STUPID SO MAKE PUBLIC METHOD TO CHANGE HEALTH IN THEH UI?
        healthbar.highValue = characterStats.health.maxHealth;
        healthbar.value = characterStats.health.currentHealth;
        healthbar.title = characterStats.health.currentHealth.ToString() + "/" + characterStats.health.maxHealth.ToString();
    }
}
