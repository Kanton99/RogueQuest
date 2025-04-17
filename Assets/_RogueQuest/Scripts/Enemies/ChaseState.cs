using UnityEngine;

// État de poursuite
namespace RogueQuest
{
    public class ChaseState : EnemyState
    {
        public ChaseState(EnemyAI enemyAI) : base(enemyAI) { }

        // Méthode appelée lors de l'entrée dans l'état de poursuite
        public override void Enter()
        {
            Debug.Log("Entering Chase State");
        }

        // Méthode appelée à chaque frame pour exécuter la logique de poursuite
        public override void Execute()
        {
            if (enemyAI == null) return;

            // Déplacement vers le joueur
            Vector2 directionToPlayer = ((Vector2)enemyAI.playerTransform.position - (Vector2)enemyAI.transform.position).normalized;
            enemyAI.transform.position += (Vector3)(directionToPlayer * enemyAI.ChaseSpeed * Time.deltaTime);

            // Détection des obstacles
            if (DetectObstacle())
            {
                // Logique pour éviter l'obstacle
                Debug.Log("Obstacle detected!");
            }

            // Détection du joueur à portée d'attaque
            if (IsPlayerInRange())
            {
                enemyAI.ChangeState(new AttackState(enemyAI));
            }
        }

        // Méthode appelée lors de la sortie de l'état de poursuite
        public override void Exit()
        {
            Debug.Log("Exiting Chase State");
        }

        // Détecter les obstacles
        private bool DetectObstacle()
        {
            RaycastHit2D hit = Physics2D.Raycast(enemyAI.transform.position, Vector2.right * enemyAI.transform.localScale.x, enemyAI.DetectionRange, enemyAI.WallLayerMask);
            return hit.collider != null && hit.collider.CompareTag("Obstacle");
        }

        // Vérifier si le joueur est à portée d'attaque
        private bool IsPlayerInRange()
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector3.down * 0.35f) + enemyAI.transform.position, enemyAI.CurrentDirection, enemyAI.AttackRange, enemyAI.WallLayerMask);
            return hit.collider != null && hit.collider.CompareTag("Player");
        }
    }
}