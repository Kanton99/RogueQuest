using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
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

	private InputSystem_Actions m_Actions;             
	private InputSystem_Actions.PlayerActions m_Player;    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
		m_Actions = new InputSystem_Actions();              // Create asset object.
		m_Player = m_Actions.Player;                      // Extract action map object.
		m_Player.AddCallbacks(this);                      // Register callback interface IPlayerActions.
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

	public void OnAttack(InputAction.CallbackContext context)
	{
		throw new System.NotImplementedException();
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
        Inventory inventory = gameObject.GetComponent<Inventory>();
        if (inventory == null){
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
		throw new System.NotImplementedException();
	}
}
