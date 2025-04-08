public abstract class Effect
{
    public float duration;
    private float timer;

    public Effect(float duration)
    {
        this.duration = duration;
        this.timer = 0f;
    }

    public bool Update(float deltaTime)
    {
        timer += deltaTime;
        return timer >= duration;
    }

    public abstract void Apply(EntityStats target);
    public abstract void Remove(EntityStats target);
}
