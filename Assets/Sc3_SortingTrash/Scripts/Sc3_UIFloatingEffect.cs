using UnityEngine;

public class Sc3_UIFloatingEffect : MonoBehaviour
{
    [Header("Floating Settings")]
    [Tooltip("ความเร็วในการลอย (ค่ามาก = ลอยเร็วขึ้น) ปรับสดได้ตอน Play")]
    public float speed = 1f;

    [Tooltip("ระยะการลอยในแนวแกน X และ Y (หน่วยพิกเซล) ปรับสดได้ตอน Play")]
    public Vector2 range = new Vector2(20f, 20f);

    [Header("Random Variation")]
    [Tooltip("ให้แต่ละ instance มีความเร็ว/เฟสต่างกันเล็กน้อย เพื่อไม่ให้ลอยพร้อมกันเป๊ะๆ ทุกตัว")]
    public bool randomizePerInstance = true;

    [Range(0f, 1f)]
    [Tooltip("เปอร์เซ็นต์ความต่างของความเร็วต่อ instance (เฉพาะเมื่อเปิด randomizePerInstance)")]
    public float speedVariance = 0.2f;

    [Header("Resume Settings")]
    [Tooltip("เวลาไล่ระดับ (fade-in) กลับเข้าสู่การลอย หลังถูก Pause/Resume เพื่อไม่ให้ตำแหน่งกระตุก")]
    public float fadeInDuration = 0.3f;

    private RectTransform rectTransform;
    private Vector2 startPos;

    private float speedMultiplierX = 1f;
    private float speedMultiplierY = 1f;
    private float phaseOffsetX;
    private float phaseOffsetY;

    private float resumeTime;
    private bool isFloating = true;
    private bool initialized = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        float variance = randomizePerInstance ? Random.Range(1f - speedVariance, 1f + speedVariance) : 1f;
        speedMultiplierX = variance;
        speedMultiplierY = variance * 0.8f;

        phaseOffsetX = Random.Range(0f, Mathf.PI * 2f);
        phaseOffsetY = Random.Range(0f, Mathf.PI * 2f);
    }

    void OnEnable()
    {
        // ไม่จับตำแหน่งตรงนี้ทันที เพราะตอน OnEnable ทำงาน
        // โค้ดที่ Spawn/วางตำแหน่ง object อาจยังไม่ทำงานเสร็จ
        // จะไปจับใน Update เฟรมแรกแทน (ตอนนั้นทุกอย่างตั้งตำแหน่งเสร็จแล้ว)
        initialized = false;
    }

    void Update()
    {
        if (!initialized)
        {
            startPos = rectTransform.anchoredPosition;
            resumeTime = Time.time;
            initialized = true;
        }

        if (!isFloating) return;

        // ไล่ระดับ amplitude จาก 0 -> เต็ม เพื่อไม่ให้ตำแหน่งกระตุกตอนเริ่ม/กลับมาลอย
        float elapsed = Time.time - resumeTime;
        float fade = fadeInDuration > 0f ? Mathf.Clamp01(elapsed / fadeInDuration) : 1f;

        float x = Mathf.Sin((Time.time * speed * speedMultiplierX) + phaseOffsetX) * range.x * fade;
        float y = Mathf.Cos((Time.time * speed * speedMultiplierY) + phaseOffsetY) * range.y * fade;

        rectTransform.anchoredPosition = startPos + new Vector2(x, y);
    }

    /// <summary>
    /// หยุดการลอยชั่วคราว (ใช้ตอนกำลังถูกลาก) เพื่อให้ตำแหน่งถูกควบคุมจากสคริปต์อื่นได้เต็มที่
    /// โดยไม่มีการแย่งเซ็ตตำแหน่งจากที่นี่
    /// </summary>
    public void PauseFloating()
    {
        isFloating = false;
    }

    /// <summary>
    /// เริ่มลอยต่อ โดยใช้ newOrigin เป็นจุดศูนย์กลางใหม่ (เช่น ตำแหน่งที่ปล่อยเมาส์)
    /// จะ fade-in เข้าสู่การลอยแบบนิ่มๆ ไม่เด้งกลับจุดเกิดเดิม
    /// </summary>
    public void ResumeFloating(Vector2 newOrigin)
    {
        startPos = newOrigin;
        resumeTime = Time.time;
        initialized = true;
        isFloating = true;
    }

    /// <summary>
    /// กำหนดจุดศูนย์กลางการลอยใหม่ทันที โดยไม่เปลี่ยนสถานะ isFloating
    /// (เผื่อกรณีโค้ดอื่น ๆ ย้าย/เทเลพอร์ต object แล้วต้องการให้ลอยรอบจุดใหม่)
    /// </summary>
    public void SetOrigin(Vector2 newOrigin)
    {
        startPos = newOrigin;
        resumeTime = Time.time;
        initialized = true;
    }

    /// <summary>
    /// คงไว้เพื่อให้เข้ากันได้กับโค้ดเดิมที่อาจเรียกใช้ชื่อนี้อยู่
    /// </summary>
    public void UpdateStartPos(Vector2 newPos)
    {
        SetOrigin(newPos);
    }
}