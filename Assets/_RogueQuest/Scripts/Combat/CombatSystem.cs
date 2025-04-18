using UnityEngine;

namespace RogueQuest
{
    public class CombatSystem : MonoBehaviour
    {
        [Header("Combat Settings")]
        public LayerMask targetLayer; // Target layer for attacks
        public float attackCooldown = 1f; // Time between attacks

        private float lastAttackTime;

        private void Update()
        {
            // Example attack triggered by pressing Space
            if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        public void Attack()
        {
            if (Time.time < lastAttackTime + attackCooldown)
            {

                return;
            }

            lastAttackTime = Time.time; // Update the last attack time

            Items.Inventory inventory = GetComponent<Items.Inventory>();
            if (inventory == null || inventory.weapon == null)
            {
                Debug.LogWarning("Aucune arme équipée pour attaquer.");
                return;
            }

            Items.Weapon equippedWeapon = inventory.weapon;

            // Detect targets within the weapon's range
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, equippedWeapon.range, targetLayer);
            foreach (Collider2D hit in hits)
            {
                // Ensure the target is not the attacker itself
                if (hit.gameObject == gameObject)
                {
                    Debug.Log("Skipping self.");
                    continue;
                }

                // Ensure the target has the "Player" tag
                if (!hit.CompareTag("Player"))
                {
                    Debug.Log($"Skipping non-player target: {hit.gameObject.name}");
                    continue;
                }

                EntityStats targetStats = hit.GetComponent<EntityStats>();
                if (targetStats != null)
                {
                    Debug.Log($"Attacking target: {hit.gameObject.name}");
                    DamageHandler.ApplyDamage(targetStats, equippedWeapon.damage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize the weapon's range in the editor
            Items.Inventory inventory = GetComponent<Items.Inventory>();
            if (inventory != null && inventory.weapon != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, inventory.weapon.range);
            }
        }
    }

    public static class DamageHandler
    {
        public static void ApplyDamage(EntityStats target, int damage)
        {
            if (target == null)
            {
                Debug.LogWarning("Cible invalide pour appliquer des dégâts.");
                return;
            }

            Debug.Log($"Applying {damage} damage to: {target.gameObject.name}");
            target.TakeDamage(damage);
        }
    }
}