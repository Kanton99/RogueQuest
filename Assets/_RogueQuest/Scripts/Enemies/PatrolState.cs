using UnityEngine;

// �tat de patrouille
namespace RogueQuest
{
    public class PatrolState : EnemyState
    {
        private Vector2 patrolDirection;
        private float changeDirectionTime;
        private float changeDirectionInterval = 1.0f; // Intervalle de changement de direction en secondes
        private Vector3 randomYMoveDirection = Vector3.up; // Pass� en private

        public PatrolState(EnemyAI enemyAI) : base(enemyAI) { }

        // M�thode appel�e lors de l'entr�e dans l'�tat de patrouille
        public override void Enter()
        {
            ChangePatrolDirection();
            changeDirectionTime = changeDirectionInterval;
        }

        // M�thode appel�e � chaque frame pour ex�cuter la logique de patrouille
        public override void Execute()
        {
            if (enemyAI == null) return;

            float moveSpeed = enemyAI.PatrolSpeed * Time.deltaTime;
            // D�placement dans la direction de patrouille
            enemyAI.transform.position += randomYMoveDirection * moveSpeed + enemyAI.CurrentDirection * moveSpeed;

            // D�tection du joueur
            if (DetectPlayer())
            {
                Debug.Log("Player detected, switching to ChaseState");
                enemyAI.ChangeState(new ChaseState(enemyAI));
            }

            // Changer de direction apr�s un certain temps ou si le sol n'est pas d�tect�
            changeDirectionTime -= Time.deltaTime;
            if (changeDirectionTime <= 0 || !IsGroundDetected())
            {
                ChangePatrolDirection();
            }
        }

        // M�thode appel�e lors de la sortie de l'�tat de patrouille
        public override void Exit()
        {
            Debug.Log("Exiting Patrol State");
        }

        // Changer la direction de patrouille
        private void ChangePatrolDirection()
        {
            randomYMoveDirection.y = Random.Range(0.5f, 1.5f);
            changeDirectionInterval = Random.Range(0.8f, 3.0f); // Changer la direction de patrouille � des intervalles al�atoires
            enemyAI.SetCurrentDirection(enemyAI.CurrentDirection * -1); // Utiliser la m�thode pour changer la direction
            changeDirectionTime = changeDirectionInterval;
        }

        // D�tecter le joueur
        private bool DetectPlayer()
        {
            RaycastHit2D hit = Physics2D.Raycast(enemyAI.CurrentDirection * 0.5f + (Vector3.down * 0.35f) + enemyAI.transform.position, enemyAI.CurrentDirection, enemyAI.DetectionRange);

            if (hit.collider != null)
            {
                Debug.Log("Raycast hit: " + hit.collider.name);
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
            return false;
        }

        // V�rifier si le sol est d�tect�
        private bool IsGroundDetected()
        {
            Vector2 origin = enemyAI.transform.position + enemyAI.CurrentDirection * 0.5f; // V�rifier l�g�rement devant l'ennemi
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, enemyAI.GroundCheckDistance);
            return hit.collider != null; // V�rifier s'il y a un sol en dessous
        }
    }
}