using UnityEngine;

public class Ladderclimbing : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 5f; // Vitesse d'escalade
    [SerializeField] private Rigidbody2D rb; // Référence au Rigidbody2D du personnage
    [SerializeField] private Animator animator; // Référence à l'Animator du personnage

    private bool isClimbing = false; // Indique si le personnage est en train d'escalader
    private bool isOnLadder = false; // Indique si le personnage est sur une échelle

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Vérifie si le joueur est sur une échelle et appuie sur les touches de montée/descente
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (isOnLadder && verticalInput != 0)
        {
            isClimbing = true;
            animator.SetBool("IsClimbing", true); // Active l'animation d'escalade

            // Détermine si le joueur monte ou descend
            animator.SetFloat("ClimbDirection", verticalInput); // Met à jour la direction dans l'Animator
        }
        else if (isClimbing && verticalInput == 0)
        {
            // Si le joueur arrête de bouger sur l'échelle
            isClimbing = false;
            animator.SetBool("IsClimbing", false); // Désactive l'animation d'escalade
        }
    }

    void FixedUpdate()
    {
        if (isClimbing)
        {
            // Déplace le personnage verticalement sur l'échelle
            float verticalInput = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
            rb.gravityScale = 0; // Désactive la gravité pendant l'escalade
        }
        else
        {
            rb.gravityScale = 1; // Réactive la gravité lorsque le personnage n'escalade pas
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder")) // Vérifie si le personnage entre en contact avec une échelle
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder")) // Vérifie si le personnage quitte l'échelle
        {
            isOnLadder = false;
            isClimbing = false;
            animator.SetBool("IsClimbing", false); // Désactive l'animation d'escalade
        }
    }
}
