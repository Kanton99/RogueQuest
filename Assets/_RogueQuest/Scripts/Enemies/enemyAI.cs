using UnityEngine;

// Classe de gestion de l'IA de l'ennemi
namespace RogueQuest
{
    public class EnemyAI : MonoBehaviour
    {
        private EnemyState currentState;

        [Header("Patrol Settings")]
        [SerializeField] private float patrolSpeed = 2.0f;
        [SerializeField] private float patrolDuration = 2.0f; // Dur�e avant de changer de direction
        private float patrolTimer = 0f;

        [Header("Chase Settings")]
        [SerializeField] private float chaseSpeed = 4.0f;

        [Header("Detection Settings")]
        [SerializeField] private float detectionRange = 5.0f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private LayerMask wallLayerMask; // LayerMask pour les murs

        [Header("Player Reference")]
        [SerializeField] public Transform playerTransform; // R�f�rence au transform du joueur

        [Header("Enemy Type")]
        [SerializeField] private EnemyType enemyType = EnemyType.SmallFry; // Type d'ennemi

        private Vector3 currentDirection = Vector3.right; // Direction actuelle de l'ennemi
        private float groundCheckDistance = 1.0f; // Distance pour v�rifier le sol

        private CombatSystem combatSystem;
        private Animator animator; // R�f�rence � l'Animator
        private Rigidbody2D rb; // R�f�rence au Rigidbody2D pour calculer la vitesse r�elle

        void Start()
        {
            combatSystem = GetComponent<CombatSystem>();
            animator = GetComponent<Animator>(); // R�cup�rer le composant Animator
            rb = GetComponent<Rigidbody2D>(); // R�cup�rer le Rigidbody2D

            // Initialiser avec l'�tat appropri� en fonction du type d'ennemi
            switch (enemyType)
            {
                case EnemyType.SmallFry:
                    ChangeState(new PatrolState(this));
                    break;
                case EnemyType.Elite:
                    ChangeState(new PatrolState(this));
                    break;
                case EnemyType.Boss:
                    ChangeState(new IdleState(this));
                    break;
            }
        }

        void Update()
        {
            if (currentState != null)
            {
                currentState.Execute();
            }

        // Check if the player is within attack range
        if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            CombatSystem combatSystem = GetComponent<CombatSystem>();
            if (combatSystem != null)
            {
                combatSystem.Attack();
            }
        }
    }

        // D�tecte si un mur est devant l'ennemi
        private bool DetectWall()
        {
            Vector2 origin = transform.position + currentDirection * 0.5f; // Point de d�part du raycast
            RaycastHit2D hit = Physics2D.Raycast(origin, currentDirection, 0.5f, wallLayerMask);
            Debug.Log("Mur d�tect� : " + (hit.collider != null)); // D�bogage
            return hit.collider != null; // Retourne true si un mur est d�tect�
        }

        // D�tecte si le sol est devant l'ennemi
        private bool DetectGround()
        {
            Vector2 origin = transform.position + currentDirection * 0.5f; // Point de d�part du raycast
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, wallLayerMask);
            Debug.Log("Sol d�tect� : " + (hit.collider != null)); // D�bogage
            return hit.collider != null; // Retourne true si le sol est d�tect�
        }

        // Changer l'�tat actuel de l'ennemi
        public void ChangeState(EnemyState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;

            if (currentState != null)
            {
                currentState.Enter();
            }
        }

        public float PatrolSpeed => patrolSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public Vector3 CurrentDirection => currentDirection; // Getter pour currentDirection
        public float GroundCheckDistance => groundCheckDistance; // Getter pour groundCheckDistance
        public LayerMask WallLayerMask => wallLayerMask; // Getter pour wallLayerMask

        // D�finir la direction actuelle de l'ennemi
        public void SetCurrentDirection(Vector3 direction)
        {
            currentDirection = direction;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            Vector2 origin = transform.position + currentDirection * 0.5f; // V�rifier l�g�rement devant l'ennemi
            Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);

            Gizmos.color = Color.red;

            Debug.DrawRay((Vector3.down * 0.35f) + transform.position, currentDirection * DetectionRange, Color.red); // Ajout d'un raycast visible pour le d�bogage
        }
    }

    public enum EnemyType
    {
        SmallFry,
        Elite,
        Boss
    }
}