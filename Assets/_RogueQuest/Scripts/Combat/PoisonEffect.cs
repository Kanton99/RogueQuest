using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    public EntityStats target;
    public float duration = 5f;
    public float tickInterval = 1f;
    public int damagePerTick = 2;

    private float timer = 0f;
    private float tickTimer = 0f;

    private void Update()
    {
        if (target == null) return;

        timer += Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            target.TakeDamage(damagePerTick);
            Debug.Log("Poison ! D�g�ts inflig�s : " + damagePerTick);
        }

        if (timer >= duration)
        {
            Destroy(this); // Supprime le poison apr�s la dur�e
        }
    }

    public static void ApplyPoison(EntityStats target, float duration, int dmg)
    {
        PoisonEffect poison = target.gameObject.AddComponent<PoisonEffect>();
        poison.target = target;
        poison.duration = duration;
        poison.damagePerTick = dmg;
    }
}
