using UnityEngine;

public class Sc2_UINotifWiggle : MonoBehaviour
{
    [Header("--- UI Slide Up Settings (เอฟเฟกต์เลื่อนเด้งขึ้น) ---")]
    [Tooltip("ความเร็วในการเลื่อนขึ้นมาจากข้างล่าง ยิ่งมากยิ่งเด้งขึ้นมาเร็ว")]
    public float slideSpeed = 5f;

    [Tooltip("ระยะเริ่มต้นที่จะให้แอบไปซ่อนใต้ตำแหน่งจริง (หน่วยเป็นพิกเซลบนจอ เช่น 500 หรือ 800)")]
    public float startYOffset = 600f;

    [Header("--- UI Wiggle Settings (เอียงซ้ายขวา) ---")]
    [Tooltip("ความเร็วในการเอียงส่ายไปมา ยิ่งน้อยยิ่งช้าและนุ่มนวล")]
    public float rotateSpeed = 2f;

    [Tooltip("องศาที่ยอมให้เอียงไปขวาและซ้ายมากที่สุด (แนะนำ 5 - 10 องศากำลังน่ารัก)")]
    public float maxRotateAngle = 7f;

    [Header("--- UI Pulse Settings (ยืดหดหายใจ) ---")]
    [Tooltip("ถ้าอยากให้ข้อความมีการขยายหดเบาๆ เหมือนหายใจได้ ให้ติ๊กถูกอันนี้ครับ")]
    public bool usePulseEffect = true;
    public float pulseSpeed = 1.5f;
    public float pulseAmount = 0.05f; // ขยายออกเพิ่มจากเดิม 5%

    private RectTransform rectTransform;
    private Vector3 initialScale;
    private Vector2 targetAnchoredPosition; // เก็บพิกัดจริงที่เราจัดไว้ในหน้าจอ UI
    private Vector2 startAnchoredPosition;  // พิกัดใต้จอสำหรับจุดเริ่มต้น
    private float randomOffset;
    private float slideProgress = 0f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = transform.localScale;

        if (rectTransform != null)
        {
            // บันทึกตำแหน่งที่แท้จริงที่คุณตั้งใจจัดวางไว้ในหน้าจอ Canvas (จุดปลายทาง)
            targetAnchoredPosition = rectTransform.anchoredPosition;

            // คำนวณจุดเกิดเริ่มต้น โดยการหักแกน Y ลงไปข้างล่างตามค่า Offset
            startAnchoredPosition = new Vector2(targetAnchoredPosition.x, targetAnchoredPosition.y - startYOffset);
        }

        // สุ่มจังหวะเริ่มต้นไม่ให้ซ้ำใคร
        randomOffset = Random.Range(0f, 100f);
    }

    void OnEnable()
    {
        slideProgress = 0f; // รีเซ็ตความคืบหน้าการเลื่อนใหม่ทุกครั้งที่เด้งขึ้นมา

        if (rectTransform != null)
        {
            // วาร์ปป้ายเตือนลงไปซ่อนไว้ใต้จอก่อนเพื่อเตรียมสไลด์ขึ้นมา
            rectTransform.anchoredPosition = startAnchoredPosition;

            // รีเซ็ตมุมกลับไปตรงๆ
            rectTransform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        if (rectTransform == null) return;

        // 🚀 1. ลอจิกค่อยๆ เลื่อนเด้งขึ้นมาจากข้างล่าง (Slide Up)
        if (slideProgress < 1f)
        {
            slideProgress += Time.deltaTime * slideSpeed;

            // ใช้ Mathf.SmoothStep เพื่อให้ตอนเริ่มเลื่อนจะเร็ว และตอนจะถึงเป้าหมายจะค่อยๆ ชะลอความเร็วลงอย่างนุ่มนวล
            float smoothT = Mathf.SmoothStep(0f, 1f, slideProgress);
            rectTransform.anchoredPosition = Vector2.Lerp(startAnchoredPosition, targetAnchoredPosition, smoothT);
        }

        // 📐 2. ลอจิกการหมุนเอียงซ้ายขวาช้าๆ (Sine Wave)
        float time = Time.time * rotateSpeed + randomOffset;
        float zRotation = Mathf.Sin(time) * maxRotateAngle;
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

        // 🎈 3. ลอจิกการยืดหดเอฟเฟกต์หายใจ (ถ้าเปิดใช้งาน)
        if (usePulseEffect)
        {
            float pulseTime = Time.time * pulseSpeed + randomOffset;
            float scaleOffset = Mathf.Sin(pulseTime) * pulseAmount;
            rectTransform.localScale = initialScale + new Vector3(scaleOffset, scaleOffset, 0f);
        }
    }
}