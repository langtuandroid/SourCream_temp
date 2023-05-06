public enum DamageTypes
{
    Physical,
    Magical,
}

public class DamageInformation
{
    public DamageTypes dmgType { get; private set; }

    public float amount { get; private set; }
    public DamageInformation(DamageTypes damageType, float value)
    {
        dmgType = damageType;
        amount = value;
    }
}