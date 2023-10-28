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

public class WeightController : MonoBehaviour
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
            new WeightedListItem<string>("attacks", 10000),
            new WeightedListItem<string>("buffs", 1),
            new WeightedListItem<string>("movementActions", 1),

        };
        actionTypeWeightedList = new(actionTypes);

        List<WeightedListItem<EnemyCombatAction>> attacksTemp = new List<WeightedListItem<EnemyCombatAction>>();
        if (behaviors.attacks.Length > 0) {
            for (int i = 0; i < behaviors.attacks.Length; i++) {
                var stringVal = behaviors.attacks[i].name;

                attacksTemp.Add(new WeightedListItem<EnemyCombatAction>(behaviors.attacks[i], 100));
            }
        }
        attacks = new(attacksTemp);

        List<WeightedListItem<EnemyCombatAction>> buffsTemp = new List<WeightedListItem<EnemyCombatAction>>();
        if (behaviors.buffs.Length > 0) {
            for (int i = 0; i < behaviors.buffs.Length; i++) {
                var stringVal = behaviors.buffs[i].name;
                buffsTemp.Add(new WeightedListItem<EnemyCombatAction>(behaviors.buffs[i], 100));
            }
        }

        buffs = new(buffsTemp);

        List<WeightedListItem<EnemyMovementAction>> movements = new List<WeightedListItem<EnemyMovementAction>>();
        {
            if (behaviors.movementActions.Length > 0) {
                for (int i = 0; i < behaviors.movementActions.Length; i++) {
                    var stringVal = behaviors.movementActions[i].name;
                    movements.Add(new WeightedListItem<EnemyMovementAction>(behaviors.movementActions[i], 100));
                }
            }
        };
        movementActions = new(movements);

        SetNextPreferedAction(false, false);
    }

    public void UpdateActionTypeWeights(Actions actionType, int preferenceAmount)
    {
        //actionTypeWeightedList
        switch (actionType) {
            case Actions.ATTACK:
                actionTypeWeightedList.SetWeight("attacks", 100 + preferenceAmount);
                break;
            case Actions.BUFF:
                actionTypeWeightedList.SetWeight("buffs", 100 + preferenceAmount);
                break;
            case Actions.MOVEMT:
                actionTypeWeightedList.SetWeight("movementActions", 100 + preferenceAmount);
                break;
            default:
                break;
        }
    }

    public void ResetActionTypeWeights()
    {
        actionTypeWeightedList.SetWeightOfAll(100);
    }

    public void ResetWeights()
    {
        attacks.SetWeightOfAll(100);
        buffs.SetWeightOfAll(100);
        movementActions.SetWeightOfAll(100);
    }

    public void UpdateWeights(Actions actionType, AttackUpdateTypes attackUpdate, int preferenceAmount)
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

    public void UpdateAttackWeights(AttackUpdateTypes attackUpdate, int preferenceAmount)
    {
        (float value, int index) largest = (0, 0);
        (float value, int index) second = (0, 0);
        var length = attacks.Count;

        switch (attackUpdate) {
            case AttackUpdateTypes.PreferRange:
                for (int i = 0; i < length; i++) {
                    if (attacks[i].range > largest.value) {
                        second = largest;
                        largest = (attacks[i].range, i);
                    } else if (attacks[i].range > second.value) {
                        second = (attacks[i].range, i);
                    }
                }
                if (attacks.Count >= 2) {
                    if (largest.value >= 10) {
                        attacks.SetWeightAtIndex(largest.index, 100 + preferenceAmount);
                    }
                    if (second.value >= 10) {
                        attacks.SetWeightAtIndex(second.index, preferenceAmount / 2);
                    }
                }
                break;
            case AttackUpdateTypes.PreferFast:
                for (int i = 0; i < length; i++) {
                    if (attacks[i].castTime > largest.value) {
                        second = largest;
                        largest = (attacks[i].castTime, i);
                    } else if (attacks[i].castTime > second.value) {
                        second = (attacks[i].castTime, i);
                    }
                }

                if (attacks.Count >= 2) {
                    attacks.SetWeightAtIndex(largest.index, 100 + preferenceAmount);
                    attacks.SetWeightAtIndex(second.index, 100 + (preferenceAmount / 2));
                }

                break;
            case AttackUpdateTypes.PreferDamage:
                for (int i = 0; i < length; i++) {
                    var damageValue = attacks[i].scallingType == ScalingTypes.PHYSICAL ? stats.attackDamge * attacks[i].value : stats.magicDamage * attacks[i].value;
                    if (damageValue > largest.value) {
                        second = largest;
                        largest = (damageValue, i);
                    } else if (damageValue > second.value) {
                        second = (damageValue, i);
                    }
                }

                if (attacks.Count >= 2) {
                    attacks.SetWeightAtIndex(largest.index, 100 + preferenceAmount);
                    attacks.SetWeightAtIndex(second.index, 100 + (preferenceAmount / 2));
                }
                break;
            default:
                break;
        }
    }

    public void UpdateBuffWeights(AttackUpdateTypes attackUpdate, int preferenceAmount)
    {
        throw new NotImplementedException();
    }

    public void UpdateMovementWeights(AttackUpdateTypes attackUpdate, int preferenceAmount)
    {
        throw new NotImplementedException();
    }

    public EnemyActions GetNextPreferedAction()
    {
        var actionTypeRoll = actionTypeWeightedList.Next();
        EnemyActions actionRoll = new EnemyActions();
        switch (actionTypeRoll) {
            case "attacks":
                actionRoll.CombatAction = attacks.Next();
                break;
            case "buffs":
                actionRoll.CombatAction = buffs.Next();
                break;
            case "movementActions":
                actionRoll.MovementAction = movementActions.Next();
                break;
        }
        nextAction = actionRoll;
        return actionRoll;

    }

    public void SetNextPreferedAction(bool resetActionTypeWeights, bool resetWeights)
    {
        nextAction = GetNextPreferedAction();

        if (resetActionTypeWeights) {
            ResetActionTypeWeights();
        }
        if (resetWeights) {
            ResetWeights();
        }
    }
}

