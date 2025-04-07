using UnityEngine;
public class ShieldEffect
{
    public int shieldValue;
    public float duration;
    private float timer;

    public ShieldEffect(int shieldValue, float duration)
    {
        this.shieldValue = shieldValue;
        this.duration = duration;
        this.timer = 0f;
    }

    public bool Update(float deltaTime)
    {
        timer += deltaTime;
        return timer >= duration || shieldValue <= 0;
    }

    public int AbsorbDamage(int damage)
    {
        int absorbed = Mathf.Min(shieldValue, damage);
        shieldValue -= absorbed;
        return absorbed;
    }
}
