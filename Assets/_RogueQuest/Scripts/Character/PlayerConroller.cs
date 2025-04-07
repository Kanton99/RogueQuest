using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isJumping;
    private bool isDashing;
    private float dashTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = new Vector2(input.x, input.y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing)
        {
            isJumping = true;
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
}
