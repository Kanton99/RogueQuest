using UnityEngine;

// Classe de gestion de l'IA de l'ennemi
namespace RogueQuest
{
    public class EnemyAI : MonoBehaviour
    {
        private EnemyState currentState;

        [Header("Patrol Settings")]
        [SerializeField] private float patrolSpeed = 2.0f;
        [SerializeField] private float patrolDuration = 2.0f; // Durée avant de changer de direction
        private float patrolTimer = 0f;

        [Header("Chase Settings")]
        [SerializeField] private float chaseSpeed = 4.0f;

        [Header("Detection Settings")]
        [SerializeField] private float detectionRange = 5.0f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private LayerMask wallLayerMask; // LayerMask pour les murs

        [Header("Player Reference")]
        [SerializeField] public Transform playerTransform; // Référence au transform du joueur

        [Header("Enemy Type")]
        [SerializeField] private EnemyType enemyType = EnemyType.SmallFry; // Type d'ennemi

        private Vector3 currentDirection = Vector3.right; // Direction actuelle de l'ennemi
        private float groundCheckDistance = 1.0f; // Distance pour vérifier le sol

        private CombatSystem combatSystem;
        private Animator animator; // Référence à l'Animator
        private Rigidbody2D rb; // Référence au Rigidbody2D pour calculer la vitesse réelle

        void Start()
        {
            combatSystem = GetComponent<CombatSystem>();
            animator = GetComponent<Animator>(); // Récupérer le composant Animator
            rb = GetComponent<Rigidbody2D>(); // Récupérer le Rigidbody2D

            // Initialiser avec l'état approprié en fonction du type d'ennemi
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

            // Logique de patrouille simple
            Patrol();

            // Check if the player is within attack range
            if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                combatSystem.Attack();
            }

            // Mettre à jour l'animation en fonction de la vitesse réelle
            float speed = rb.velocity.magnitude; // Calculer la vitesse actuelle
            animator.SetFloat("Speed", speed);

            // Flip du sprite en fonction de la direction
            FlipSpriteBasedOnDirection();
        }

        // Logique de patrouille simple
        private void Patrol()
        {
            patrolTimer += Time.deltaTime;

            // Alterne la direction toutes les X secondes
            if (patrolTimer >= patrolDuration || DetectWall() || !DetectGround())
            {
                patrolTimer = 0f;
                SetCurrentDirection(-currentDirection); // Inverse la direction
                Debug.Log("Direction inversée : " + currentDirection); // Débogage
            }

            // Applique la vitesse de patrouille
            rb.velocity = new Vector2(currentDirection.x * patrolSpeed, rb.velocity.y);
            Debug.Log("Vitesse appliquée : " + rb.velocity); // Débogage
        }

        // Méthode pour gérer le flip du sprite
        private void FlipSpriteBasedOnDirection()
        {
            Debug.Log("Direction pour le flip : " + currentDirection.x); // Débogage

            if (currentDirection.x > 0) // Si l'ennemi se déplace vers la droite
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Regarder à droite
            }
            else if (currentDirection.x < 0) // Si l'ennemi se déplace vers la gauche
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Regarder à gauche
            }
        }

        // Détecte si un mur est devant l'ennemi
        private bool DetectWall()
        {
            Vector2 origin = transform.position + currentDirection * 0.5f; // Point de départ du raycast
            RaycastHit2D hit = Physics2D.Raycast(origin, currentDirection, 0.5f, wallLayerMask);
            Debug.Log("Mur détecté : " + (hit.collider != null)); // Débogage
            return hit.collider != null; // Retourne true si un mur est détecté
        }

        // Détecte si le sol est devant l'ennemi
        private bool DetectGround()
        {
            Vector2 origin = transform.position + currentDirection * 0.5f; // Point de départ du raycast
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, wallLayerMask);
            Debug.Log("Sol détecté : " + (hit.collider != null)); // Débogage
            return hit.collider != null; // Retourne true si le sol est détecté
        }

        // Changer l'état actuel de l'ennemi
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

        // Définir la direction actuelle de l'ennemi
        public void SetCurrentDirection(Vector3 direction)
        {
            currentDirection = direction;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            Vector2 origin = transform.position + currentDirection * 0.5f; // Vérifier légèrement devant l'ennemi
            Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);

            Gizmos.color = Color.red;

            Debug.DrawRay((Vector3.down * 0.35f) + transform.position, currentDirection * DetectionRange, Color.red); // Ajout d'un raycast visible pour le débogage
        }
    }

    public enum EnemyType
    {
        SmallFry,
        Elite,
        Boss
    }
}