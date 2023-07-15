
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
    void UpdateWeights(Actions actionType, AttackUpdateTypes attackUpdate, float preferenceAmount);

    void UpdateBuffWeights(AttackUpdateTypes attackUpdate, float preferenceAmount);
    void UpdateAttackWeights(AttackUpdateTypes attackUpdate, float preferenceAmount);
    void UpdateMovementWeights(AttackUpdateTypes attackUpdate, float preferenceAmount);
    EnemyActions getNextPreferedAction();



    //Something with enemy(main character) state
}

