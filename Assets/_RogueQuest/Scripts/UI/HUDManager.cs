using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Barre de Vie")]
    public Slider healthSlider;

    [Header("Quickslots")]
    public List<Image> quickSlotIcons = new List<Image>(); // Associez les icônes des quickslots dans l'inspecteur.

    [Header("Buffs et Debuffs")]
    public Transform buffContainer; // Conteneur pour les icônes de buffs/debuffs.
    public GameObject buffIconPrefab; // Préfabriqué pour une icône de buff/debuff.

    private EntityStats playerStats;

    private void Start()
    {
        // Assurez-vous que le joueur est assigné.
        playerStats = FindObjectOfType<EntityStats>();
        if (playerStats == null)
        {
            Debug.LogError("Aucun EntityStats trouvé pour le joueur !");
            return;
        }

        // Initialiser la barre de vie.
        healthSlider.maxValue = playerStats.GetMaxHealth();
        healthSlider.value = playerStats.currentHealth;
    }

    private void Update()
    {
        if (playerStats == null) return;

        // Mettre à jour la barre de vie.
        healthSlider.maxValue = playerStats.GetMaxHealth();
        healthSlider.value = playerStats.currentHealth;

        // Mettre à jour les buffs et debuffs.
        UpdateBuffIcons();
    }

    public void UpdateQuickSlot(int slotIndex, Sprite icon)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotIcons.Count)
        {
            Debug.LogWarning("Index de quickslot invalide !");
            return;
        }

        quickSlotIcons[slotIndex].sprite = icon;
        quickSlotIcons[slotIndex].enabled = icon != null; // Désactive l'icône si aucun sprite n'est assigné.
    }

    private void UpdateBuffIcons()
    {
        // Supprimer toutes les icônes existantes.
        foreach (Transform child in buffContainer)
        {
            Destroy(child.gameObject);
        }

        // Ajouter des icônes pour chaque effet actif.
        foreach (var effect in playerStats.GetActiveEffects())
        {
            if (effect is StatModifier statModifier)
            {
                GameObject icon = Instantiate(buffIconPrefab, buffContainer);
                // Configurez l'icône (par exemple, changez l'image ou le texte).
                icon.GetComponentInChildren<Text>().text = statModifier.statType.ToString();
            }
        }
    }
}
