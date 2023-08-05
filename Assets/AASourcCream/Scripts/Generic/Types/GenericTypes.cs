public enum ScalingTypes
{
    PHYSICAL,
    MAGICAL,
}

public enum AttackType
{
    INSTANT,
    CHANNEL,
    RANGEINSTANT,
    PROJECTILE
}

public enum TargetTypes
{
    ENEMY,
    ALLY,
    SELF,
    LOCATION,
}

public enum CastType
{
    MELEE,
    RANGE
}

public enum MovementDirections
{
    FORWARD,
    SIDEWAY,
    BACKWARD,
}
public class DamageInformation
{
    public ScalingTypes dmgType { get; private set; }

    public float amount { get; private set; }
    public DamageInformation(ScalingTypes damageType, float value)
    {
        dmgType = damageType;
        amount = value;
    }
}