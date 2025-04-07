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
        target.ApplyShield(shieldValue, duration);
    }

    public override void Remove(EntityStats target)
    {
        target.RemoveShield();
    }

    public int AbsorbDamage(int damage)
    {
        int absorbed = Mathf.Min(shieldValue, damage);
        shieldValue -= absorbed;
        return absorbed;
    }
}
