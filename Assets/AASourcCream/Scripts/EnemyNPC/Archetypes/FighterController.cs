using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles stuff
public class FighterController : MonoBehaviour
{
    private WeightController weightController;

    private StatsComponent statsComponent;

    // Start is called before the first frame update
    void Start()
    {
        weightController = GetComponent<WeightController>();
        statsComponent = GetComponent<StatsComponent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAbilityFinished()
    {
        var shouldReset = false;
        if (statsComponent.health.currentHealth < (statsComponent.health.maxHealth / 2)) {
            weightController.UpdateActionTypeWeights(Actions.BUFF, 1000);
            shouldReset = true;
        } else {
            weightController.UpdateActionTypeWeights(Actions.ATTACK, 10000);
            shouldReset = true;
        }
        weightController.SetNextPreferedAction(shouldReset, false);
    }
    //Adjust weights based on stats
}
