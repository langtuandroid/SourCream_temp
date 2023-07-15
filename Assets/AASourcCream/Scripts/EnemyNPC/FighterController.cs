using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct EnemyActions
{
    public EnemyCombatAction CombatAction { get; set; }
    public EnemyMovementAction MovementAction { get; set; }
}
public enum Actions
{
    ATTACK,
    BUFF,
    MOVEMT
}

public enum AttackUpdateTypes
{
    PreferRange,
    PreferFast,
    PreferDamage,
}

public class FighterController : MonoBehaviour, BehaviorController
{
    public StatsComponent stats { get; set; }
    public EnemyDataType behaviors { get; set; }

    public WeightedList<EnemyCombatAction> attacks { get; set; }
    public WeightedList<EnemyCombatAction> buffs { get; set; }
    public WeightedList<EnemyMovementAction> movementActions { get; set; }
    public WeightedList<string> actionTypeWeightedList { get; set; }
    public EnemyActions nextAction { get; private set; }

    public void Start()
    {
        stats = GetComponent<StatsComponent>();
        behaviors = GetComponent<EnemyDataController>()?.enemyData;
        InitializeWeights();
    }

    // Should initialization depend on stats/state?
    public void InitializeWeights()
    {
        List<WeightedListItem<string>> actionTypes = new() {
            new WeightedListItem<string>("attacks", 900),
            new WeightedListItem<string>("buffs", 1),
            new WeightedListItem<string>("movementActions", 1),

        };
        actionTypeWeightedList = new(actionTypes);

        List<WeightedListItem<EnemyCombatAction>> attacksTemp = new List<WeightedListItem<EnemyCombatAction>>();
        if (behaviors.attacks.Length > 0) {
            for (int i = 0; i < behaviors.attacks.Length; i++) {
                var stringVal = behaviors.attacks[i].name;
                attacksTemp.Add(new WeightedListItem<EnemyCombatAction>(behaviors.attacks[i], 1));
            }
        }
        attacks = new(attacksTemp);

        List<WeightedListItem<EnemyCombatAction>> buffsTemp = new List<WeightedListItem<EnemyCombatAction>>();
        if (behaviors.buffs.Length > 0) {
            for (int i = 0; i < behaviors.buffs.Length; i++) {
                var stringVal = behaviors.buffs[i].name;
                buffsTemp.Add(new WeightedListItem<EnemyCombatAction>(behaviors.buffs[i], 1));
            }
        }

        buffs = new(buffsTemp);

        List<WeightedListItem<EnemyMovementAction>> movements = new List<WeightedListItem<EnemyMovementAction>>();
        {
            if (behaviors.movementActions.Length > 0) {
                for (int i = 0; i < behaviors.movementActions.Length; i++) {
                    var stringVal = behaviors.movementActions[i].name;
                    movements.Add(new WeightedListItem<EnemyMovementAction>(behaviors.movementActions[i], 1));
                }
            }
        };
        movementActions = new(movements);

        setNextPreferedAction();
    }


    public void UpdateWeights(Actions actionType, AttackUpdateTypes attackUpdate, float preferenceAmount)
    {
        switch (actionType) {
            case Actions.ATTACK:
                UpdateAttackWeights(attackUpdate, preferenceAmount);
                break;
            case Actions.BUFF:
                UpdateBuffWeights(attackUpdate, preferenceAmount);
                break;
            case Actions.MOVEMT:
                UpdateMovementWeights(attackUpdate, preferenceAmount);
                break;
        }

    }

    public void UpdateAttackWeights(AttackUpdateTypes attackUpdate, float preferenceAmount)
    {
        switch (attackUpdate) {
            case AttackUpdateTypes.PreferRange:
                var lenght = attacks.Count;
                for (int i = 0; i < lenght; i++) {
                    //TODO
                }
                break;
            case AttackUpdateTypes.PreferFast:
                break;
            case AttackUpdateTypes.PreferDamage:
                break;
            default:
                break;
        }


        // List<WeightedListItem<string>> attacksTemp = new List<WeightedListItem<string>>();
        // if (behaviors.attacks.Length > 0) {
        //     for (int i = 0; i < behaviors.attacks.Length; i++) {
        //         var stringVal = behaviors.attacks[i].name;
        //         attacksTemp.Add(new WeightedListItem<string>(stringVal, 1));
        //     }
        // }

        // attacks = new(attacksTemp);
    }

    public void UpdateBuffWeights(AttackUpdateTypes attackUpdate, float preferenceAmount)
    {
        throw new NotImplementedException();
    }

    public void UpdateMovementWeights(AttackUpdateTypes attackUpdate, float preferenceAmount)
    {
        throw new NotImplementedException();
    }

    public EnemyActions getNextPreferedAction()
    {
        var actionTypeRoll = actionTypeWeightedList.Next();
        Debug.Log("ROLLING");
        return actionTypeRoll switch {
            "attacks" => new EnemyActions { CombatAction = attacks.Next() },
            "buffs" => new EnemyActions { CombatAction = buffs.Next() },
            "movementActions" => new EnemyActions { MovementAction = movementActions.Next() }
        };

    }

    public void setNextPreferedAction()
    {
        nextAction = getNextPreferedAction();
    }
}

