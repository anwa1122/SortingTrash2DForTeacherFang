using UnityEngine;

public class Sc2_PlayerMovement : MonoBehaviour
{
    [Header("Player-Setting")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;


    [Header("Ground-Check-Setting")]
    // [SerializeField] private ยังแก้ใน Inspector ได้ Script อื่นเข้ามายุ่งไม่ได้
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;




    //Others//----------------------------------------------------------------------
    private Rigidbody2D rb;

    private Vector2 moveInput;

    //AllWork//----------------------------------------------------------------------
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

        //Debug.Log(isGrounded);

        moveInput.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Jumped");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

}