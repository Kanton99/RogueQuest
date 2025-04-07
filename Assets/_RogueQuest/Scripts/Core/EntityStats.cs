using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [Header("Stats de Base")]
    public int baseMaxHealth = 100;
    public int baseAttackPower = 10;
    public int baseDefense = 5;
    public float baseMoveSpeed = 5f;

    public int currentHealth;

    private List<Effect> activeEffects = new List<Effect>();
    private ShieldEffect currentShield;

    private void Awake()
    {
        currentHealth = baseMaxHealth;
    }

    private void Update()
    {
        if (activeEffects.Count > 0)
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                if (activeEffects[i].Update(Time.deltaTime))
                {
                    activeEffects[i].Remove(this);
                    activeEffects.RemoveAt(i);
                }
            }
        }

        if (currentShield != null && currentShield.Update(Time.deltaTime))
        {
            currentShield = null;
        }
    }

    public int GetAttack()
    {
        float total = baseAttackPower;
        foreach (var effect in activeEffects)
        {
            if (effect is StatModifier mod && mod.statType == StatType.Attack)
            {
                total += mod.value;
            }
        }

        return Mathf.RoundToInt(total);
    }

    public float GetMoveSpeed()
    {
        float total = baseMoveSpeed;
        foreach (var effect in activeEffects)
        {
            if (effect is StatModifier mod && mod.statType == StatType.MoveSpeed)
            {
                total += mod.value;
            }
        }

        return total;
    }

    public int GetDefense()
    {
        float total = baseDefense;
        foreach (var effect in activeEffects)
        {
            if (effect is StatModifier mod && mod.statType == StatType.Defense)
            {
                total += mod.value;
            }
        }

        return Mathf.RoundToInt(total);
    }

    public int GetMaxHealth()
    {
        float total = baseMaxHealth;
        foreach (var effect in activeEffects)
        {
            if (effect is StatModifier mod && mod.statType == StatType.MaxHealth)
            {
                total += mod.value;
            }
        }

        return Mathf.RoundToInt(total);
    }

    public void ApplyEffect(Effect effect)
    {
        effect.Apply(this);
        activeEffects.Add(effect);
    }

    public void AddStatModifier(StatModifier modifier)
    {
        activeEffects.Add(modifier);
    }

    public void RemoveStatModifier(StatModifier modifier)
    {
        activeEffects.Remove(modifier);
    }

    public void ApplyShield(int value, float duration)
    {
        currentShield = new ShieldEffect(value, duration);
        ApplyEffect(currentShield);
    }

    public void RemoveShield()
    {
        currentShield = null;
    }

    public void TakeDamage(int damage)
    {
        if (currentShield != null)
        {
            int absorbed = currentShield.AbsorbDamage(damage);
            damage -= absorbed;

            Debug.Log($"Bouclier a absorbé {absorbed} dégâts.");
        }

        int defense = GetDefense();
        int damageTaken = Mathf.Max(damage - defense, 1);
        currentHealth -= damageTaken;

        Debug.Log($"Dégâts restants : {damageTaken}. Vie : {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Entité morte");
        // Death logic...
    }
}
