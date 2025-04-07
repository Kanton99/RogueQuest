public enum StatType
{
    Attack,
    Defense,
    MoveSpeed,
    MaxHealth
}

public class StatModifier
{
    public StatType statType;
    public float value;
    public float duration; // Durée en secondes
    private float timer;

    public StatModifier(StatType statType, float value, float duration)
    {
        this.statType = statType;
        this.value = value;
        this.duration = duration;
        this.timer = 0f;
    }

    public bool UpdateTimer(float deltaTime)
    {
        timer += deltaTime;
        return timer >= duration;
    }
}
