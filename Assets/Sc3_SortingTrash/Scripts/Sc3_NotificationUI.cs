using System.Collections;
using UnityEngine;
using TMPro; // สำหรับใช้งาน TextMeshPro

public class Sc3_NotificationUI : MonoBehaviour
{
    public static Sc3_NotificationUI Instance;

    [Header("--- 1. ระบบแจ้งเตือนขยะไม่ครบ (Slide & Fade) ---")]
    [Tooltip("ลาก GameObject กล่องเตือนที่มี CanvasGroup มาใส่ช่องนี้")]
    public CanvasGroup targetUI;
    public float fadeSpeed = 5f;
    public float moveSpeed = 8f;
    public float slideOffset = 50f;
    public float displayDuration = 2f;

    [Header("--- 2. ระบบหน้าต่างสรุปผล (Zoom & Fade In) ---")]
    [Tooltip("ลาก RectTransform ของหน้าต่างสรุปผลสี่เหลี่ยมผืนผ้าแนวตั้งมาใส่ช่องนี้")]
    public RectTransform summaryWindow;
    [Tooltip("ลาก CanvasGroup ของหน้าต่างสรุปผลมาใส่ช่องนี้ (เอาไว้ทำล่องหนตอนเริ่ม)")]
    public CanvasGroup summaryCanvasGroup;
    [Tooltip("ความเร็วในการซูมขยายใหญ่และการเฟดสว่าง")]
    public float zoomSpeed = 8f;
    [Tooltip("ขนาดปลายทางที่อยากให้ขยายจนสุด (ปกติคือ 1, 1, 1)")]
    public Vector3 targetScale = Vector3.one;

    private RectTransform targetRect;
    private TextMeshProUGUI warningText;
    private Vector2 openedPosition;
    private Vector2 closedPosition;

    private Coroutine currentRoutine;
    private Coroutine summaryRoutine; // Coroutine แยกสำหรับหน้าต่างสรุปผล

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // ตั้งค่าระบบแจ้งเตือน Slide & Fade
        if (targetUI != null)
        {
            targetRect = targetUI.GetComponent<RectTransform>();
            warningText = targetUI.GetComponentInChildren<TextMeshProUGUI>();

            if (warningText == null)
            {
                Debug.LogWarning($"[Notice UI] คำเตือน: ไม่พบ Component TextMeshProUGUI ในออบเจกต์ {targetUI.name} หรือลูกๆ ของมันเลยนะครับ!");
            }

            targetUI.alpha = 0f;
            targetUI.blocksRaycasts = false;

            if (targetRect != null)
            {
                openedPosition = targetRect.anchoredPosition;
                closedPosition = openedPosition - new Vector2(0f, slideOffset);
                targetRect.anchoredPosition = closedPosition;
            }
        }

        // 🔥 ตั้งค่าหน้าต่างสรุปผลให้หดเหลือ 0 และ ล่องหน (Alpha = 0) ทันทีตอนเริ่มเกม
        if (summaryWindow != null)
        {
            summaryWindow.localScale = Vector3.zero;
        }

        if (summaryCanvasGroup != null)
        {
            summaryCanvasGroup.alpha = 0f;
            summaryCanvasGroup.blocksRaycasts = false; // ป้องกันไม่ให้เมาส์ไปกดโดนปุ่มข้างในตอนที่ยังมองไม่เห็น
        }
    }

    // ==========================================
    // [หมวดที่ 1] ระบบแจ้งเตือนขยะไม่ครบ
    // ==========================================
    public void ShowNotice(string message)
    {
        if (targetUI == null || targetRect == null) return;

        if (warningText != null)
        {
            warningText.text = message;
        }

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        targetUI.alpha = 0f;
        targetRect.anchoredPosition = closedPosition;

        currentRoutine = StartCoroutine(NoticeSequence());
    }

    private IEnumerator NoticeSequence()
    {
        targetUI.blocksRaycasts = true;

        while (targetUI.alpha < 0.99f)
        {
            targetUI.alpha = Mathf.Lerp(targetUI.alpha, 1f, Time.deltaTime * fadeSpeed);
            targetRect.anchoredPosition = Vector2.Lerp(targetRect.anchoredPosition, openedPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
        targetUI.alpha = 1f;
        targetRect.anchoredPosition = openedPosition;

        yield return new WaitForSeconds(displayDuration);

        while (targetUI.alpha > 0.01f)
        {
            targetUI.alpha = Mathf.Lerp(targetUI.alpha, 0f, Time.deltaTime * fadeSpeed);
            targetRect.anchoredPosition = Vector2.Lerp(targetRect.anchoredPosition, closedPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
        targetUI.alpha = 0f;
        targetRect.anchoredPosition = closedPosition;
        targetUI.blocksRaycasts = false;
    }

    // ==========================================
    // [หมวดที่ 2] ระบบหน้าต่างสรุปผล (ซูมพร้อมเฟดเข้า)
    // ==========================================
    public void ShowSummaryWindow()
    {
        if (summaryWindow == null || summaryCanvasGroup == null)
        {
            Debug.LogError("[Summary UI] กรุณาลากหน้าต่างสรุปผลและ CanvasGroup มาใส่ให้ครบก่อนสั่งทำงานครับ!");
            return;
        }

        if (summaryRoutine != null)
        {
            StopCoroutine(summaryRoutine);
        }

        summaryRoutine = StartCoroutine(ZoomInSummarySequence());
    }

    // IEnumerator สำหรับซูมขยายและเฟดหน้าต่างสรุปผลจากกลางจอ
    private IEnumerator ZoomInSummarySequence()
    {
        // รีเซ็ตค่าเริ่มต้นก่อนเริ่มอนิเมชันเพื่อความชัวร์
        summaryWindow.localScale = Vector3.zero;
        summaryCanvasGroup.alpha = 0f;
        summaryCanvasGroup.blocksRaycasts = true; // เปิดให้สามารถกดปุ่มต่างๆ บนหน้าต่างสรุปผลได้แล้ว

        // ค่อยๆ ขยายขนาดและเพิ่มความสว่างไปพร้อมๆ กัน
        while (Vector3.Distance(summaryWindow.localScale, targetScale) > 0.001f || summaryCanvasGroup.alpha < 0.99f)
        {
            // ซูมขนาด
            summaryWindow.localScale = Vector3.Lerp(summaryWindow.localScale, targetScale, Time.deltaTime * zoomSpeed);

            // เฟดความสว่าง (ใช้ความเร็ว zoomSpeed ร่วมกันเพื่อให้สว่างเสร็จพร้อมกับตอนขยายสุดพอดี)
            summaryCanvasGroup.alpha = Mathf.Lerp(summaryCanvasGroup.alpha, 1f, Time.deltaTime * zoomSpeed);

            yield return null;
        }

        // ล็อกค่าสุดท้ายให้เป๊ะ 100%
        summaryWindow.localScale = targetScale;
        summaryCanvasGroup.alpha = 1f;
        Debug.Log("[Summary UI] หน้าต่างสรุปผลขยายใหญ่และเฟดสว่างเรียบร้อยแล้ว!");
    }
}