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
    public bool isBuff => value > 0; // D�termine si c'est un buff (valeur positive) ou un debuff (valeur n�gative).

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
