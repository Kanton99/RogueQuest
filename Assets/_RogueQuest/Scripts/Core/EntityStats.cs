using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;   

public class EntityStats : MonoBehaviour
{
    [Header("Stats de Base")]
    public int baseMaxHealth = 100;
    public int baseAttackPower = 10;
    public int baseDefense = 5;
    public float baseMoveSpeed = 5f;

    private int _currentHealthBackingField;
    public int currentHealth
    {
        get => _currentHealthBackingField;
        set
        {
            if (_currentHealthBackingField != value)
            {
                Debug.Log($"Current health of {gameObject.name} changed from {_currentHealthBackingField} to {value}");
                _currentHealthBackingField = value;
                OnHealthChanged?.Invoke(currentHealth, GetMaxHealth());
                Debug.Log($"OnHealthChanged event invoked for {gameObject.name}");
            }
        }
    }

    public event Action<int, int> OnHealthChanged; // Event to notify health changes

    private List<Effect> activeEffects = new List<Effect>();

    [Header("Game Over Canvas")]
    public GameObject gameOverCanvas;

    private void Awake()
    {
        currentHealth = baseMaxHealth;
        Debug.Log($"Awake called on {gameObject.name}. Current health set to: {currentHealth}");
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
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

    public void TakeDamage(int damage)
    {
        int defense = GetDefense();
        int damageTaken = Mathf.Max(damage - defense, 0);
        currentHealth -= damageTaken;

        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"TakeDamage called on {gameObject.name}. Damage: {damage}, Remaining Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyEffect(Effect effect)
    {
        effect.Apply(this);
        activeEffects.Add(effect);
        Debug.Log($"Effect applied: {effect.GetType().Name} to {gameObject.name}");
    }

    public void AddStatModifier(StatModifier modifier)
    {
        activeEffects.Add(modifier);
        Debug.Log($"Added {modifier.statType} modifier with value {modifier.value} to {gameObject.name}.");
    }

    public void RemoveStatModifier(StatModifier modifier)
    {
        activeEffects.Remove(modifier);
        Debug.Log($"Removed {modifier.statType} modifier with value {modifier.value} from {gameObject.name}.");
    }

    public List<Effect> GetActiveEffects()
    {
        return activeEffects;
    }

    private void Die()
    {
        Debug.Log("Entité morte");
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Game Over Canvas is not assigned.");
        }
    }
}
