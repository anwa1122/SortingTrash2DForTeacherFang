using UnityEngine;
using UnityEngine.SceneManagement;

public class Sc2_PlayerManager : MonoBehaviour
{
    [Header("Player-Setting")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public bool canMove = true;

    [Header("Jump Physics Tweaks (เพิ่มความแน่น)")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

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
        // 🔒 [สเต็ปที่ 1: ดัก Update] ถ้ากำลังนับถอยหลังเปิดเกม ห้ามรับปุ่มกดใดๆ และเบรกแกน X ทันที ส่วนแกน Y ปล่อยให้ร่วงตามปกติ
        if (Sc2_CountdownTimer.IsIntroCounting)
        {
            moveInput = Vector2.zero; // ล้างค่าปุ่มกดทิ้งทั้งหมดเป็น 0

            if (rb != null)
            {
                // 🔥 บังคับล็อกเฉพาะแกน X เป็น 0 ส่วนแกน Y ใช้ค่าเดิมเพื่อให้ตัวละครร่วงลงพื้นตามฟิสิกส์ได้ครับ
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
            return; // หักห้ามใจไม่ให้รันลอจิกรับปุ่มกดกระโดดหรือกด E ด้านล่าง
        }

        // ตรวจสอบพื้นตามปกติ
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // รับค่าปุ่มเดิน
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // --- 🎮 ระบบกระโดด ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = true;
            }
            else if (!isGrounded && canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false;
            }
        }

        // --- 🎒 ระบบกด E ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentTrash != null)
            {
                Sc2_InventoryManager.Instance.AddItem(currentTrash.GetComponent<Sc2_TrashObject>().data);
                Destroy(currentTrash);
            }
            else if (canTeleport)
            {
                LoadingScreen.LoadSceneWithLoadingScreen("Sc3_SortingTrash");
            }
        }
    }

    private void FixedUpdate()
    {
        // 🔒 [สเต็ปที่ 2: ดัก FixedUpdate] บังคับล็อกขาในระบบฟิสิกส์ด้วย เพื่อตัดปัญหาแรงเฉื่อยไถลซ้ายขวา
        if (Sc2_CountdownTimer.IsIntroCounting)
        {
            if (rb != null)
            {
                // บังคับล็อก X สนิทกริบ ส่วน Y ปล่อยร่วงอิสระ
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
            return; // สั่งหยุดทำงานลอจิกเคลื่อนที่ด้านล่างทันที
        }

        // 🏃‍♂️ ระบบคุมความเร็วเดินปกติ (ทำงานเฉพาะตอนเกมเริ่มแล้ว)
        if (moveInput.x != 0)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // 📐 ลอจิกฟิสิกส์คุมแรงโน้มถ่วงอัจฉริยะ (ยังคงรันได้ปกติแม้ตอนล็อกขา ทำให้ร่วงลงพื้นได้นุ่มนวลมาก)
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
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