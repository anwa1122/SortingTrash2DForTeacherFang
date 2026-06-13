using UnityEngine;
using UnityEngine.SceneManagement;

public class Sc2_PlayerManager : MonoBehaviour
{
    [Header("Player-Setting")]
    public float moveSpeed = 8f;            // 💡 แนะนำปรับเพิ่มความเร็วเดิน (เดิม 5f อาจจะช้าไป)
    public float jumpForce = 12f;           // 💡 ปรับเพิ่มแรงส่งกระโดดให้รับกับแรงโน้มถ่วงใหม่

    [Header("Jump Physics Tweaks (เพิ่มความแน่น)")]
    public float fallMultiplier = 2.5f;     // 🚀 แรงดึงลงตอนตก (ยิ่งมากยิ่งตกเร็ว สะใจ)
    public float lowJumpMultiplier = 2f;    // 🚀 แรงดึงลงตอนปล่อยปุ่มกระโดดเร็ว (ช่วยให้กดกระโดดสั้น-ยาวได้)

    [Header("Ground-Check-Setting")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    private bool canDoubleJump;

    private bool canTeleport;
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
        // ตรวจสอบพื้นตามปกติ
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // รับค่าปุ่มเดิน
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // --- 🎮 ระบบกระโดด ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // 💡 เปลี่ยนมาเซ็ตความเร็วแกน Y ตรงๆ จะเสถียรกว่า AddForce
                canDoubleJump = true;
            }
            else if (!isGrounded && canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false;
            }
        }

        // --- 🎒 ระบบกด E (แยกเงื่อนไขเพื่อไม่ให้บั๊กซ้อนกัน) ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentTrash != null)
            {
                Sc2_InventoryManager.Instance.AddItem(currentTrash.GetComponent<Sc2_TrashObject>().data);
                Destroy(currentTrash);
            }
            else if (canTeleport)
            {
                // 🌟 ปรับไปวาร์ปแบบผ่านคัตซีนหัวใจโหลดผ่าน LoadingScreen ตัวกลางที่เราเพิ่งแก้กันเมื่อกี้ได้เลย!
                LoadingScreen.LoadSceneWithLoadingScreen("Sc3_SortingTrash");
            }
        }
    }

    private void FixedUpdate()
    {
        // 🏃‍♂️ ระบบคุมความเร็วเดิน
        if (moveInput.x != 0)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // 🛑 ถ้าปล่อยปุ่มเดิน ให้หักความเร็ว X เป็น 0 ทันที ตัวละครจะเบรกกริบ ไม่ลอยไถล
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // 📐 ลอจิกฟิสิกส์คุมแรงโน้มถ่วงอัจฉริยะ (แก้ปัญหากระโดดลอยเคว้ง)
        if (rb.linearVelocity.y < 0) // ตอนกำลังร่วงลงมา
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space)) // แตะปุ่ม Spacebar เบาๆ (Low Jump)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trash")) currentTrash = other.gameObject;
        if (other.CompareTag("Teleporter")) canTeleport = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Trash")) currentTrash = null;
        if (other.CompareTag("Teleporter")) canTeleport = false;
    }
}