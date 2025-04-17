using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueQuest
{
    public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;

        [Header("Cooldown Settings")]
        public float jumpCooldown = 1f; // Durée du cooldown pour le saut

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private bool isJumping;
        private bool isDashing;
        private float dashTime;
        private float jumpCooldownTimer; // Timer pour le cooldown du saut

        private InputSystem_Actions m_Actions;
        private InputSystem_Actions.PlayerActions m_Player;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            m_Actions = new InputSystem_Actions(); // Create asset object.
            m_Player = m_Actions.Player; // Extract action map object.
            m_Player.AddCallbacks(this); // Register callback interface IPlayerActions.
        }

        void OnDestroy()
        {
            m_Actions.Dispose();
        }

        void OnEnable()
        {
            m_Player.Enable();
        }

        void OnDisable()
        {
            m_Player.Disable();
        }

        private void Update()
        {
            if (isDashing)
            {
                dashTime -= Time.deltaTime;
                if (dashTime <= 0)
                {
                    isDashing = false;
                }
            }

            // Réduire le timer du cooldown du saut
            if (jumpCooldownTimer > 0)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            if (isDashing)
            {
                rb.linearVelocity = new Vector2(moveInput.x * dashSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

                if (isJumping)
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    isJumping = false;
                }
            }

            // Inverser le flip du sprite pour correspondre à la direction du mouvement
            if (moveInput.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1); // Inversé pour regarder à droite
            }
            else if (moveInput.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1); // Inversé pour regarder à gauche
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            moveInput = new Vector2(input.x, input.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // Vérifier si le saut est possible (pas de cooldown actif)
            if (context.performed && !isDashing && !isJumping && jumpCooldownTimer <= 0)
            {
                isJumping = true;
                jumpCooldownTimer = jumpCooldown; // Réinitialiser le cooldown
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed && !isDashing)
            {
                isDashing = true;
                dashTime = dashDuration;
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Définir une portée d'attaque
                float attackRange = 1.5f;
                int attackDamage = 10;

                // Trouver les ennemis dans la portée
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

                foreach (Collider2D enemy in hitEnemies)
                {
                    // Vérifier si l'objet touché a un composant EntityStats et n'est pas le joueur lui-même
                    EntityStats enemyStats = enemy.GetComponent<EntityStats>();
                    if (enemyStats != null && enemy.gameObject != gameObject)
                    {
                        // Infliger des dégâts à l'ennemi
                        enemyStats.TakeDamage(attackDamage);
                        Debug.Log($"Ennemi touché ! Vie restante : {enemyStats.currentHealth}");
                    }
                }

                // Ajouter des effets visuels ou sonores ici
                Debug.Log("Attaque effectuée !");
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            Items.Inventory inventory = gameObject.GetComponent<Items.Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning("No Inventory");
                return;
            }
            inventory.PickUpItem();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Augmenter la vitesse de déplacement pour le sprint
                moveSpeed *= 1.5f; // Par exemple, augmenter de 50%
            }
            else if (context.canceled)
            {
                // Réinitialiser la vitesse de déplacement
                moveSpeed /= 1.5f; // Revenir à la vitesse normale
            }
        }
    }
}