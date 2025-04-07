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
        target.ApplyShieldEffect(this);
    }

    public override void Remove(EntityStats target)
    {
        target.RemoveShieldEffect(this);
    }

    public int AbsorbDamage(int damage)
    {
        int absorbed = Mathf.Min(shieldValue, damage);
        shieldValue -= absorbed;
        return absorbed;
    }
}
