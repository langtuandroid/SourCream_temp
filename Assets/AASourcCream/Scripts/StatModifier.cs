using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public enum StatModifierType
{
    HEALTH,
    ARMOUR,
}

public enum StatModifierUnit
{
    PERCENT,
    FLAT,
}

[System.Serializable]
public class StatModifier
{
    private StatModifierType type;
    private StatModifierUnit unit;
    private float value = 1.0f;
    private float maxValue = 2.0f;
    private float minValue = 0.5f;

    private float lastModifyTime = Time.time;
    private float lastModifyDuration = 0.0f;

    private List<Timer> timers = new List<Timer>();

    public StatModifier(StatModifierType type, float value)
    {
        this.type = type;
        this.value = value;
    }

    public float GetValue()
    {
        return this.value;
    }

    public void AddToModifier(float value, float duration)
    {
        this.lastModifyTime = Time.time;
        this.lastModifyDuration = duration;

        this.value += value;
        Timer t = new Timer();
        t.Interval = duration * 1000;
        t.AutoReset = false; // Stops it from repeating
        t.Elapsed += (sender, e) => UndoAddModifier(sender, e, value);
        t.Start();
        this.timers.Add(t);
        //Debug.Log(this.value);
    }

    public void InstantModifier(float value)
    {
        this.value += value;
    }

    private void UndoAddModifier(object sender, ElapsedEventArgs e, float modValue)
    {
        this.value -= modValue;
        //Debug.Log(this.value);
    }
}
