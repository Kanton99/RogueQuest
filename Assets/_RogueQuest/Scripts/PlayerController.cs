using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public bool isJumping = false;
    public float jumpForce;
    private bool isGrounded = true; // Ajout d'une variable pour vérifier si le joueur est au sol
    public float dashForce = 10f; // Ajout d'une variable pour la puissance du dash
    public float dashCooldown = 3f; // Durée minimale entre deux dashs
    private float lastDashTime = -Mathf.Infinity; // Temps du dernier dash, initialisé à -∞

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }

        Vector3 movement = new Vector3(horizontalInput, 0f, 0f) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        if (Input.GetButtonDown("Jump") && isGrounded) // Vérifie si le joueur est au sol avant de sauter
        {
            isJumping = true;
            Debug.Log("je saute");
        }

        if (isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
            isGrounded = false; // Le joueur n'est plus au sol après un saut
        }

        // Dash avec cooldown
        if (Input.GetKeyDown(KeyCode.LeftShift) && horizontalInput != 0 && Time.time >= lastDashTime + dashCooldown)
        {
            rb.AddForce(new Vector2(horizontalInput * dashForce, 0f), ForceMode2D.Impulse);
            lastDashTime = Time.time; // Met à jour le temps du dernier dash
            Debug.Log("Dash !");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si le joueur touche le sol
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true; // Réinitialise l'état au sol
        }
    }
}
