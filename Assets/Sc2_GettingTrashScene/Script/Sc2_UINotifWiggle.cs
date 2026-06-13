using UnityEngine;

public class Sc2_UINotifWiggle : MonoBehaviour
{
    [Header("--- UI Slam Zoom Settings (เอฟเฟกต์โยนทุบกระแทกหน้าจอ) ---")]
    public float startZoomScale = 3.5f;
    public float slamSpeed = 8f;
    public float startSlamAngle = -25f;

    [Header("--- UI Slide Up Settings (เฉพาะข้อความภารกิจตอนเปิดตัวเริ่มต้น) ---")]
    public float slideSpeed = 5f;
    public float startYOffset = 600f;

    [Header("--- UI Wiggle Settings (ลูปส่ายชิลๆ ประจำบ้าน) ---")]
    public float rotateSpeed = 2f;
    public float maxRotateAngle = 7f;

    [Header("--- UI Pulse Settings (ลูปหายใจยืดหด) ---")]
    public bool usePulseEffect = true;
    public float pulseSpeed = 1.5f;
    public float pulseAmount = 0.05f;

    private RectTransform rectTransform;
    private Vector3 initialScale;
    private Vector2 targetAnchoredPosition;
    private Vector2 startAnchoredPosition;
    private float randomOffset;

    private float slideProgress = 1f;
    private float slamProgress = 1f;
    private bool isSlamming = false;
    private bool isSliding = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = transform.localScale;

        if (rectTransform != null)
        {
            targetAnchoredPosition = rectTransform.anchoredPosition;
            startAnchoredPosition = new Vector2(targetAnchoredPosition.x, targetAnchoredPosition.y - startYOffset);
        }
        randomOffset = Random.Range(0f, 100f);
    }

    // 🚀 ลบคำสั่ง OnEnable เจ้าปัญหาออกถาวร เพื่อให้รันได้ราบรื่นไม่ตีกันเองตามคิวงาน

    public void StartSlideUpEffect()
    {
        isSlamming = false;
        isSliding = true;
        slideProgress = 0f;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startAnchoredPosition;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = initialScale;
        }
    }

    public void StartSlamZoomEffect()
    {
        isSliding = false;
        isSlamming = true;
        slamProgress = 0f; // รีเซ็ตตัวคูณแอนิเมชันให้เริ่มนับใหม่ทุกครั้งที่สลับตัวเลข

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = targetAnchoredPosition;
            rectTransform.localScale = initialScale * startZoomScale;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, startSlamAngle);
        }
    }

    void Update()
    {
        if (rectTransform == null) return;

        // ลอจิกระบบกระแทกหน้าจอ (Slam Zoom)
        if (isSlamming && slamProgress < 1f)
        {
            slamProgress += Time.deltaTime * slamSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, slamProgress);
            rectTransform.localScale = Vector3.Lerp(initialScale * startZoomScale, initialScale, smoothT);
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startSlamAngle, 0f, smoothT));

            if (slamProgress >= 1f) isSlamming = false;
            return;
        }

        // ลอจิกระบบสไลด์ขึ้นจากข้างล่าง (Slide Up)
        if (isSliding && slideProgress < 1f)
        {
            slideProgress += Time.deltaTime * slideSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, slideProgress);
            rectTransform.anchoredPosition = Vector2.Lerp(startAnchoredPosition, targetAnchoredPosition, smoothT);

            if (slideProgress >= 1f) isSliding = false;
            return;
        }

        // ระบบลูปดุ๊กดิ๊กแกว่งตัวนุ่มๆ และหายใจพองยุบ (ทำงานปกติเมื่อเอฟเฟกต์เปิดตัวจบลงแล้ว)
        float time = Time.time * rotateSpeed + randomOffset;
        float zRotation = Mathf.Sin(time) * maxRotateAngle;
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

        if (usePulseEffect)
        {
            float pulseTime = Time.time * pulseSpeed + randomOffset;
            float scaleOffset = Mathf.Sin(pulseTime) * pulseAmount;
            rectTransform.localScale = initialScale + new Vector3(scaleOffset, scaleOffset, 0f);
        }
    }
}