using UnityEngine;

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
        Inventory inventory = GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogWarning("No Inventory component found on this GameObject.");
            return;
        }

        if (inventory.weapon == null)
        {
            Debug.LogWarning("Aucune arme équipée pour attaquer.");
            return;
        }

        Weapon equippedWeapon = inventory.weapon;

        // Detect targets within the weapon's range
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
        // Visualize the weapon's range in the editor
        Inventory inventory = GetComponent<Inventory>();
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
    /// Applique des dégâts à une entité.
    /// </summary>
    /// <param name="target">L'entité cible.</param>
    /// <param name="damage">Les dégâts infligés.</param>
    public static void ApplyDamage(EntityStats target, int damage)
    {
        if (target == null)
        {
            Debug.LogWarning("Cible invalide pour appliquer des dégâts.");
            return;
        }

        target.TakeDamage(damage);
    }
}
