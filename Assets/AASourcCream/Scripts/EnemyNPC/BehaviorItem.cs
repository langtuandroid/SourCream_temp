using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public class BehaviorItem
{
    public string name;
    public int weight;
    public int range;
    public TargetTypes targetType;
    public BehaviorData behaviorTypes;


}

public class BehaviorData
{

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
