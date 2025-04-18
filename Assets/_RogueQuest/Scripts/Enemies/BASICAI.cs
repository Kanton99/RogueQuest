using UnityEngine;

public class BASICAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2.0f; // Vitesse de déplacement
    [SerializeField] private Transform groundCheck; // Point de vérification du sol
    [SerializeField] private float groundCheckDistance = 0.5f; // Distance pour vérifier le sol
    [SerializeField] private LayerMask groundLayer; // Layer des tiles "Ground"
    [SerializeField] private float directionChangeCooldown = 0.5f; // Temps minimum entre deux inversions de direction
    [SerializeField] private float patrolRange = 5.0f; // Distance maximale de patrouille

    [Header("Animator")]
    [SerializeField] private Animator animator; // Référence à l'Animator

    private Rigidbody2D rb; // Référence au Rigidbody2D
    private Vector2 direction = Vector2.left; // Direction initiale (gauche)
    private bool canChangeDirection = true; // Contrôle si le PNJ peut changer de direction
    private float directionChangeTimer = 0f; // Timer pour gérer le délai entre les inversions
    private Vector2 startPosition; // Position de départ du PNJ

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        startPosition = transform.position; // Enregistre la position de départ
    }

    void Update()
    {
        Patrol();
        FlipSprite();
        HandleDirectionChangeCooldown();
    }

    private void Patrol()
    {
        // Vérifie si le sol est devant ou si la distance maximale est atteinte
        if (canChangeDirection && (!DetectGround() || HasReachedPatrolLimit()))
        {
            direction = -direction; // Inverse la direction
            canChangeDirection = false; // Désactive le changement de direction temporairement
            directionChangeTimer = directionChangeCooldown; // Réinitialise le timer
        }

        // Applique la vitesse de déplacement
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        // Met à jour l'animation de course
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    private bool DetectGround()
    {
        // Vérifie si le sol est présent devant le PNJ
        Vector2 origin = groundCheck.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null; // Retourne true si le sol est détecté
    }

    private bool HasReachedPatrolLimit()
    {
        // Vérifie si le PNJ a dépassé la distance maximale de patrouille
        float distanceFromStart = Vector2.Distance(startPosition, transform.position);
        return distanceFromStart >= patrolRange;
    }

    private void FlipSprite()
    {
        // Retourne le sprite en fonction de la direction
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(0.13f, 0.13f, transform.localScale.z); // Regarder à droite
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-0.13f, 0.13f, transform.localScale.z); // Regarder à gauche
        }
    }

    private void HandleDirectionChangeCooldown()
    {
        // Gère le délai entre les inversions de direction
        if (!canChangeDirection)
        {
            directionChangeTimer -= Time.deltaTime;
            if (directionChangeTimer <= 0f)
            {
                canChangeDirection = true; // Réactive le changement de direction
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dessine le raycast pour la détection du sol
        Gizmos.color = Color.cyan;
        if (groundCheck != null)
        {
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        // Dessine la limite de patrouille
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, startPosition + Vector2.right * patrolRange);
        Gizmos.DrawLine(startPosition, startPosition + Vector2.left * patrolRange);
    }
}
