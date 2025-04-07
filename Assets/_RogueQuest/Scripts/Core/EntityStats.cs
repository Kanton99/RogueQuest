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

    public void ApplyModifier(StatModifier modifier)
    {
        activeModifiers.Add(modifier);
    }

    public void TakeDamage(int damage)
    {
        int defense = baseDefense;
        foreach (var mod in activeModifiers)
            if (mod.statType == StatType.Defense)
                defense += (int)mod.value;

        int damageTaken = Mathf.Max(damage - defense, 1);
        currentHealth -= damageTaken;

        Debug.Log($"Dégâts subis : {damageTaken} (Défense : {defense}). Vie restante : {currentHealth}");

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
