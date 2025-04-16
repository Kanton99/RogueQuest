using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityStats : MonoBehaviour
{
    [Header("Stats de Base")]
    public int baseMaxHealth = 100;
    public int baseAttackPower = 10;
    public int baseDefense = 5;
    public float baseMoveSpeed = 5f;

    public int currentHealth;

    private List<Effect> activeEffects = new List<Effect>();

    [Header("Game Over Canvas")]
    public GameObject gameOverCanvas; // Reference to the Game Over Canvas

    private void Awake()
    {
        currentHealth = baseMaxHealth;
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false); // Ensure the Game Over Canvas is initially hidden
        }
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

    public void ApplyTemporaryModifier(StatType statType, float value, float duration)
    {
        StatModifier modifier = new StatModifier(statType, value, duration);
        ApplyEffect(modifier);
    }

    public void ApplyPermanentModifier(StatType statType, float value)
    {
        StatModifier modifier = new StatModifier(statType, value, float.MaxValue);
        ApplyEffect(modifier);
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
        int defense = GetDefense();
        int damageTaken = Mathf.Max(damage - defense, 0);
        currentHealth -= damageTaken;

        Debug.Log($"Dégâts restants : {damageTaken}. Vie : {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Ensure health does not go negative
            Die();
        }
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
            gameOverCanvas.SetActive(true); // Show the Game Over Canvas
        }
        else
        {
            Debug.LogWarning("Game Over Canvas is not assigned.");
        }
        // Additional death logic, such as stopping the game or reloading the scene
        // For example, you can reload the scene after a delay:
        // StartCoroutine(ReloadSceneAfterDelay(2f));
    }

    // Optional: Coroutine to reload the scene after a delay
    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
