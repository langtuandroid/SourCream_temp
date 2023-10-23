using System;
using MessagePack;

[MessagePackObject]
public class EnemyDataType
{
    [Key(0)]
    public string name { get; set; }
    [Key(1)]
    public int healthPoints { get; set; }
    [Key(2)]
    public int manaPoints { get; set; }
    [Key(3)]
    public int attackDamage { get; set; }
    [Key(4)]
    public EnemyCombatAction[] attacks { get; set; }
    [Key(5)]
    public EnemyCombatAction[] buffs { get; set; }
    [Key(6)]
    public EnemyMovementAction[] movementActions { get; set; }


}

// [MessagePackObject]
// [Serializable]
public class EnemyCombatAction
{
    [Key(0)]
    public string name;
    [Key(1)]
    public ScalingTypes scallingType;
    [Key(2)]
    public AttackType attackType;
    [Key(3)]
    public float range;
    [Key(4)]
    public float value;
    [Key(5)]
    public float castTime;
    [Key(6)]
    public TargetTypes target;
}

[MessagePackObject]
[Serializable]
public class EnemyMovementAction
{
    [Key(0)]
    public string name;
    [Key(1)]
    public float distance;
    [Key(2)]
    public MovementDirections direction;
}



// ENEMY THE UNIELDING WOODEN DUMMY OF DOOM

// HP 1000
// MANA 200
// ATTACK DAMAGE 10
// ACTIONS 
// -ATTACKS
// -- REGULAR SWING
// --- ATTACK TYPE - PHYSICAL
// --- ATTACK TIMER - INSTANT
// --- ATTACK RANGE - 100?
// --- VALUE - 1.2 x AD
// --- CAST TIME - 0.5s
// -- SHOOT HOOK
// --- ATTACK TYPE - MAGICAL
// --- ATTACK TIMER - INSTANT
// --- ATTACK RANGE - 300?
// --- VALUE - 1.2 x MAGICAL EFFICIENCY?
// --- CAST TIME - 1s 
// -- BUFFS
// --- TARGET - SELF
// --- VALUEE - 1.5 x MAGICAL EFFICIENCY?
// --- ATTACK TIMER - 2 TICKS
// --- CAST TIME - 2s
// --- CAST TYPE - CHANNEL
// -- MOVEMENT
// --- DISTANCE - 200?
// --- DIRECTION - SIDEWAY