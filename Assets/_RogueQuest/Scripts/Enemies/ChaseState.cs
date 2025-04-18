using UnityEngine;

// �tat de poursuite
namespace RogueQuest
{
    public class ChaseState : EnemyState
    {
        public ChaseState(EnemyAI enemyAI) : base(enemyAI) { }

        // M�thode appel�e lors de l'entr�e dans l'�tat de poursuite
        public override void Enter()
        {
            Debug.Log("Entering Chase State");
        }

        // M�thode appel�e � chaque frame pour ex�cuter la logique de poursuite
        public override void Execute()
        {
            if (enemyAI == null) return;

            // D�placement vers le joueur
            Vector2 directionToPlayer = ((Vector2)enemyAI.playerTransform.position - (Vector2)enemyAI.transform.position).normalized;
            enemyAI.transform.position += (Vector3)(directionToPlayer * enemyAI.ChaseSpeed * Time.deltaTime);

            // D�tection des obstacles
            if (DetectObstacle())
            {
                // Logique pour �viter l'obstacle
                Debug.Log("Obstacle detected!");
            }

            // D�tection du joueur � port�e d'attaque
            if (IsPlayerInRange())
            {
                enemyAI.ChangeState(new AttackState(enemyAI));
            }
        }

        // M�thode appel�e lors de la sortie de l'�tat de poursuite
        public override void Exit()
        {
            Debug.Log("Exiting Chase State");
        }

        // D�tecter les obstacles
        private bool DetectObstacle()
        {
            RaycastHit2D hit = Physics2D.Raycast(enemyAI.transform.position, Vector2.right * enemyAI.transform.localScale.x, enemyAI.DetectionRange, enemyAI.WallLayerMask);
            return hit.collider != null && hit.collider.CompareTag("Obstacle");
        }

        // V�rifier si le joueur est � port�e d'attaque
        private bool IsPlayerInRange()
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector3.down * 0.35f) + enemyAI.transform.position, enemyAI.CurrentDirection, enemyAI.AttackRange, enemyAI.WallLayerMask);
            return hit.collider != null && hit.collider.CompareTag("Player");
        }
    }
}