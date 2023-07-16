
public interface BehaviorController
{
    StatsComponent stats { get; set; }
    EnemyDataType behaviors { get; set; }

    EnemyActions nextAction { get; }

    WeightedList<EnemyCombatAction> attacks { get; set; }
    WeightedList<EnemyCombatAction> buffs { get; set; }
    WeightedList<EnemyMovementAction> movementActions { get; set; }
    WeightedList<string> actionTypeWeightedList { get; set; }

    void InitializeWeights();
    void UpdateWeights(Actions actionType, AttackUpdateTypes attackUpdate, int preferenceAmount);

    void UpdateBuffWeights(AttackUpdateTypes attackUpdate, int preferenceAmount);
    void UpdateAttackWeights(AttackUpdateTypes attackUpdate, int preferenceAmount);
    void UpdateMovementWeights(AttackUpdateTypes attackUpdate, int preferenceAmount);

    void UpdateActionTypeWeights(Actions actionType, int preferenceAmount);
    EnemyActions getNextPreferedAction();

    void ResetActionTypeWeights();
    void ResetWeights();

    //Something with enemy(main character) state
}

