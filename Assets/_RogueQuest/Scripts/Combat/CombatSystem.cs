using UnityEngine;


namespace RogueQuest
{
    public class CombatSystem : MonoBehaviour
    {
        [Header("Combat Settings")]
        public LayerMask targetLayer; // Cible des attaques (ennemis, joueur, etc.)
        public float attackCooldown = 1f; // Temps entre deux attaques

        private float lastAttackTime;

        private void Update()
        {
            // Exemple d'attaque d�clench�e par une touche (Espace)
            if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        public void Attack()
        {
            Items.Inventory inventory = GetComponent<Items.Inventory>();
            if (inventory == null || inventory.weapon == null)
            {
                Debug.LogWarning("Aucune arme �quip�e pour attaquer.");
                return;
            }

            Items.Weapon equippedWeapon = inventory.weapon;

            // D�tection des cibles dans la port�e de l'arme
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, equippedWeapon.range, targetLayer);
            foreach (Collider2D hit in hits)
            {
                EntityStats targetStats = hit.GetComponent<EntityStats>();
                if (targetStats != null && hit.gameObject != gameObject) // Ensure the target is not the attacker
                {
                    DamageHandler.ApplyDamage(targetStats, equippedWeapon.damage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Affiche la port�e de l'arme �quip�e dans l'�diteur
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
        /// <summary>
        /// Applique des d�g�ts � une entit�.
        /// </summary>
        /// <param name="target">L'entit� cible.</param>
        /// <param name="damage">Les d�g�ts inflig�s.</param>
        public static void ApplyDamage(EntityStats target, int damage)
        {
            if (target == null)
            {
                Debug.LogWarning("Cible invalide pour appliquer des d�g�ts.");
                return;
            }

            target.TakeDamage(damage);
        }
    }
}