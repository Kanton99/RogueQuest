using static UnityEngine.GraphicsBuffer;

public enum StatType
{
    Attack,
    Defense,
    MoveSpeed,
    MaxHealth
}

public class StatModifier : Effect
{
    public StatType statType;
    public float value;

    public StatModifier(StatType statType, float value, float duration) : base(duration)
    {
        this.statType = statType;
        this.value = value;
    }

    public override void Apply(EntityStats target)
    {
        target.AddStatModifier(this);
    }

    public override void Remove(EntityStats target)
    {
        target.RemoveStatModifier(this);
    }
}
