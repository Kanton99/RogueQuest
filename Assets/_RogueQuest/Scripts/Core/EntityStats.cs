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

    private List<StatModifier> activeModifiers = new List<StatModifier>();
    private ShieldEffect currentShield;

    private void Awake()
    {
        currentHealth = baseMaxHealth;
    }

    private void Update()
    {
        if (activeModifiers.Count > 0)
        {
            for (int i = activeModifiers.Count - 1; i >= 0; i--)
            {
                if (activeModifiers[i].UpdateTimer(Time.deltaTime))
                {
                    activeModifiers.RemoveAt(i);
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
        foreach (var mod in activeModifiers)
            if (mod.statType == StatType.Attack)
                total += mod.value;

        return Mathf.RoundToInt(total);
    }

    public float GetMoveSpeed()
    {
        float total = baseMoveSpeed;
        foreach (var mod in activeModifiers)
            if (mod.statType == StatType.MoveSpeed)
                total += mod.value;

        return total;
    }

    public int GetDefense()
    {
        float total = baseDefense;
        foreach (var mod in activeModifiers)
            if (mod.statType == StatType.Defense)
                total += mod.value;

        return Mathf.RoundToInt(total);
    }

    public int GetMaxHealth()
    {
        float total = baseMaxHealth;
        foreach (var mod in activeModifiers)
            if (mod.statType == StatType.MaxHealth)
                total += mod.value;

        return Mathf.RoundToInt(total);
    }

    public void ApplyModifier(StatModifier modifier)
    {
        activeModifiers.Add(modifier);
    }

    public void ApplyModifiers(List<StatModifier> modifiers)
    {
        foreach (var modifier in modifiers)
        {
            ApplyModifier(modifier);
        }
    }

    public void RemoveModifier(StatModifier modifier)
    {
        activeModifiers.Remove(modifier);
    }

    public void ResetModifiers()
    {
        activeModifiers.Clear();
    }

    public bool HasActiveModifiers()
    {
        return activeModifiers.Count > 0;
    }

    public void ApplyShield(int value, float duration)
    {
        currentShield = new ShieldEffect(value, duration);
    }

    public void ApplyTemporaryModifier(StatType statType, float value, float duration)
    {
        StatModifier modifier = new StatModifier(statType, value, duration);
        ApplyModifier(modifier);
    }

    public void ApplyPermanentModifier(StatType statType, float value)
    {
        StatModifier modifier = new StatModifier(statType, value, float.MaxValue);
        ApplyModifier(modifier);
    }

    public void ApplySpeedBoost(float value, float duration)
    {
        ApplyTemporaryModifier(StatType.MoveSpeed, value, duration);
    }

    public void ApplyAttackBoost(float value, float duration)
    {
        ApplyTemporaryModifier(StatType.Attack, value, duration);
    }

    public void ApplyDefenseBoost(float value, float duration)
    {
        ApplyTemporaryModifier(StatType.Defense, value, duration);
    }

    public void ApplyHealthBoost(float value, float duration)
    {
        ApplyTemporaryModifier(StatType.MaxHealth, value, duration);
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
