using UnityEngine;

// Classe de gestion de l'IA de l'ennemi
public class EnemyAI : MonoBehaviour
{
    private EnemyState currentState;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 2.0f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 4.0f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5.0f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Player Reference")]
    [SerializeField] public Transform playerTransform; // Référence au transform du joueur

    private Vector3 currentDirection = Vector3.right; // Passé en private
    private float groundCheckDistance = 1.0f; // Passé en private

    void Start()
    {
        // Initialiser avec l'état de patrouille
        ChangeState(new PatrolState(this));
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
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
    public Vector3 CurrentDirection => currentDirection; // Ajouté un getter pour currentDirection
    public float GroundCheckDistance => groundCheckDistance; // Ajouté un getter pour groundCheckDistance

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
