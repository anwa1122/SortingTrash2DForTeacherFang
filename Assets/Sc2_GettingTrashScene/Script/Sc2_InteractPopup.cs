using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sc2_InteractPopup : MonoBehaviour
{
    // เปลี่ยนชื่อเป็น Instance ประจำซีน (สคริปต์อื่นในซีนเดียวกันเรียกใช้ง่าย)
    public static Sc2_InteractPopup Instance { get; private set; }

    [Header("--- UI Bounce Settings (เด้งจากกลางขึ้นบน) ---")]
    [Tooltip("ระยะความสูงที่อยากให้เด้งขึ้นไปแนวตั้งตอนเปิดตัว")]
    public float bounceYOffset = 100f;
    [Tooltip("ความเร็วในการเด้ง")]
    public float bounceSpeed = 5f;

    [Header("--- UI Hide Settings (สั่งหายตัว) ---")]
    [Tooltip("ความเร็วในการหดและจางหายไป")]
    public float hideSpeed = 6f;

    [Header("--- UI Wiggle Settings (ลูปส่ายชิลๆ ประจำบ้าน) ---")]
    public float rotateSpeed = 2f;
    public float maxRotateAngle = 5f;

    [Header("--- UI Pulse Settings (ลูปหายใจยืดหด) ---")]
    public bool usePulseEffect = true;
    public float pulseSpeed = 1.5f;
    public float pulseAmount = 0.04f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 initialScale;
    private Vector2 targetAnchoredPosition;
    private float randomOffset;

    private bool isBouncing = false;
    private bool isHiding = false;
    private Coroutine activeCoroutine;

    void Awake()
    {
        Instance = this; // ผูกสัมพันธ์ประจำซีนนี้

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        initialScale = transform.localScale;

        if (rectTransform != null)
        {
            targetAnchoredPosition = rectTransform.anchoredPosition;
        }
        randomOffset = Random.Range(0f, 100f);

        // ปิดการมองเห็นไว้ก่อนตอนเริ่มเกม จะได้ไม่โผล่มาคาจอ
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 🟢 สั่งให้ป้าย "กด E" เด้งดึ๋งพุ่งขึ้นมาโชว์ตัว (เรียกใช้เมื่อเดินเข้าใกล้ขยะ)
    /// </summary>
    public void ShowPopup()
    {
        ResetAllStates();
        isBouncing = true;
        gameObject.SetActive(true);
        activeCoroutine = StartCoroutine(BounceUpRoutine());
    }

    /// <summary>
    /// 🔴 สั่งให้ป้าย "กด E" หดหายตัวไป (เรียกใช้เมื่อเดินห่างออกไป หรือเก็บขยะชิ้นนั้นเสร็จแล้ว)
    /// </summary>
    public void HidePopup()
    {
        if (!gameObject.activeInHierarchy || isHiding) return;
        ResetAllStates();
        isHiding = true;
        activeCoroutine = StartCoroutine(HideRoutine());
    }

    private void ResetAllStates()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        isBouncing = false;
        isHiding = false;
        if (canvasGroup != null) canvasGroup.alpha = 1f;
        if (rectTransform != null) rectTransform.localScale = initialScale;
    }

    IEnumerator BounceUpRoutine()
    {
        float progress = 0f;
        Vector2 startPos = new Vector2(targetAnchoredPosition.x, targetAnchoredPosition.y - bounceYOffset);

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPos;
            rectTransform.localScale = Vector3.zero;
        }

        while (progress < 1f)
        {
            progress += Time.deltaTime * bounceSpeed;
            float t = Mathf.SmoothStep(0f, 1f, progress);
            float bounceScale = Mathf.Sin(t * Mathf.PI * 1.5f) * 0.1f / (1f + progress * 2f);

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetAnchoredPosition, t);
                rectTransform.localScale = initialScale * (t + bounceScale);
            }
            yield return null;
        }

        if (rectTransform != null) rectTransform.anchoredPosition = targetAnchoredPosition;
        isBouncing = false;
    }

    IEnumerator HideRoutine()
    {
        float progress = 0f;
        Vector3 currentScale = rectTransform.localScale;

        while (progress < 1f)
        {
            progress += Time.deltaTime * hideSpeed;
            float t = Mathf.SmoothStep(0f, 1f, progress);

            if (rectTransform != null) rectTransform.localScale = Vector3.Lerp(currentScale, Vector3.zero, t);
            if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        gameObject.SetActive(false);
        isHiding = false;
    }

    void Update()
    {
        if (rectTransform == null || isBouncing || isHiding) return;

        // ดุ๊กดิ๊กหายใจตอนโชว์ตัวปกติ
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