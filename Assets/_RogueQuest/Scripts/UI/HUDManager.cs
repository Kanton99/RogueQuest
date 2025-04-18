using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RogueQuest
{
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
        [Header("Quickslot Placeholder")]
        public Sprite placeholderSprite;

        private EntityStats playerStats;
        private Dictionary<StatType, Sprite> statTypeToSprite;

        private void Start()
        {
            playerStats = Object.FindFirstObjectByType<EntityStats>();
            if (playerStats == null)
            {
                Debug.LogError("Aucun EntityStats trouvé pour le joueur !");
                return;
            }

            // Subscribe to the OnHealthChanged event
            playerStats.OnHealthChanged += UpdateHealthBar;
            Debug.Log("HUDManager successfully subscribed to OnHealthChanged event");

            // Initialize the health bar
            healthSlider.maxValue = playerStats.GetMaxHealth();
            healthSlider.value = playerStats.currentHealth;
        }

        private void OnDestroy()
        {
            // Unsubscribe from the event to avoid memory leaks
            if (playerStats != null)
            {
                playerStats.OnHealthChanged -= UpdateHealthBar;
            }
        }

        private void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            Debug.Log($"HUD updated: Current Health = {currentHealth}, Max Health = {maxHealth}");
        }

        private void UpdateBuffIcons()
        {
            foreach (Transform child in buffContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var effect in playerStats.GetActiveEffects())
            {
                if (effect is StatModifier statModifier)
                {
                    GameObject icon = Instantiate(buffIconPrefab, buffContainer);

                    Image iconImage = icon.GetComponentInChildren<Image>();
                    if (iconImage != null && statTypeToSprite.ContainsKey(statModifier.statType))
                    {
                        iconImage.sprite = statTypeToSprite[statModifier.statType];
                        iconImage.color = statModifier.isBuff ? Color.green : Color.red;
                    }

                    Text iconText = icon.GetComponentInChildren<Text>();
                    if (iconText != null)
                    {
                        iconText.text = $"{(statModifier.isBuff ? "+" : "-")}{Mathf.Abs(statModifier.value)}";
                    }
                }
            }
        }

        public void UpdateQuickSlots(Items.Inventory inventory)
        {
            for (int i = 0; i < quickSlotIcons.Count; i++)
            {
                if (i < inventory.consumables.Length && inventory.consumables[i] != null)
                {
                    quickSlotIcons[i].sprite = inventory.consumables[i].sprite;
                    quickSlotIcons[i].color = Color.white;
                }
                else
                {
                    quickSlotIcons[i].sprite = placeholderSprite;
                    quickSlotIcons[i].color = new Color(1, 1, 1, 0.5f);
                }
            }
        }
    }
}
