using UnityEngine;

public class Sc2_PlayerMovement : MonoBehaviour
{
    [Header("Player-Setting")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground-Check-Setting")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    // เก็บขยะ
    private GameObject currentTrash;

    // Others
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        moveInput.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Jumped");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // กด E เพื่อเก็บขยะ
        if (currentTrash != null && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(currentTrash);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trash"))
        {
            currentTrash = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Trash"))
        {
            currentTrash = null;
        }
    }
}