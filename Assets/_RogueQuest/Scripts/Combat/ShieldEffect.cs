using UnityEngine;
public class ShieldEffect : Effect
{
    public int shieldValue;

    public ShieldEffect(int shieldValue, float duration) : base(duration)
    {
        this.shieldValue = shieldValue;
    }

    public override void Apply(EntityStats target)
    {
        // Appliquer un modificateur de défense temporaire
        StatModifier defenseModifier = new StatModifier(StatType.Defense, shieldValue, duration);
        target.ApplyEffect(defenseModifier);
    }

    public override void Remove(EntityStats target)
    {
        // Supprimer le modificateur de défense temporaire
        StatModifier defenseModifier = new StatModifier(StatType.Defense, shieldValue, duration);
        target.RemoveStatModifier(defenseModifier);
    }
}
