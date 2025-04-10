using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Barre de Vie")]
    public Slider healthSlider;

    [Header("Quickslots")]
    public List<Image> quickSlotIcons = new List<Image>();

    [Header("Buffs et Debuffs")]
    public Transform buffContainer;
    public GameObject buffIconPrefab;

    [Header("Sprites pour Buffs/Debuffs")]
    public Sprite attackBuffSprite;
    public Sprite defenseBuffSprite;
    public Sprite moveSpeedBuffSprite;
    public Sprite maxHealthBuffSprite;

    private EntityStats playerStats;
    private Dictionary<StatType, Sprite> statTypeToSprite;

    private void Start()
    {
        // Assurez-vous que le joueur est assigné.
        playerStats = Object.FindFirstObjectByType<EntityStats>();
        if (playerStats == null)
        {
            Debug.LogError("Aucun EntityStats trouvé pour le joueur !");
            return;
        }

        // Initialiser la barre de vie.
        healthSlider.maxValue = playerStats.GetMaxHealth();
        healthSlider.value = playerStats.currentHealth;

        // Initialiser le dictionnaire des sprites.
        statTypeToSprite = new Dictionary<StatType, Sprite>
        {
            { StatType.Attack, attackBuffSprite },
            { StatType.Defense, defenseBuffSprite },
            { StatType.MoveSpeed, moveSpeedBuffSprite },
            { StatType.MaxHealth, maxHealthBuffSprite }
        };
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

                // Assigner le sprite correspondant au StatType.
                Image iconImage = icon.GetComponentInChildren<Image>();
                if (iconImage != null && statTypeToSprite.ContainsKey(statModifier.statType))
                {
                    iconImage.sprite = statTypeToSprite[statModifier.statType];
                    // Appliquer une couleur différente pour les buffs et debuffs.
                    iconImage.color = statModifier.isBuff ? Color.green : Color.red;
                }

                // Optionnel : Ajouter du texte pour afficher des informations supplémentaires.
                Text iconText = icon.GetComponentInChildren<Text>();
                if (iconText != null)
                {
                    iconText.text = $"{(statModifier.isBuff ? "+" : "-")}{Mathf.Abs(statModifier.value)}";
                }
            }
        }
    }
}
