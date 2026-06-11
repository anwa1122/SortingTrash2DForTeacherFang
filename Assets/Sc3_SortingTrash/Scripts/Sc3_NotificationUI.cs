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

    [Header("--- 2. ระบบหน้าต่างสรุปผล (Zoom Scale) ---")]
    [Tooltip("ลาก RectTransform ของหน้าต่างสรุปผลสี่เหลี่ยมผืนผ้าแนวตั้งมาใส่ช่องนี้")]
    public RectTransform summaryWindow;
    [Tooltip("ความเร็วในการซูมขยายใหญ่")]
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

        // 🔥 ตั้งค่าหน้าต่างสรุปผลให้หดเหลือ 0 รอไว้ตั้งแต่เริ่มเกม
        if (summaryWindow != null)
        {
            summaryWindow.localScale = Vector3.zero;
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
    // [หมวดที่ 2] ระบบหน้าต่างสรุปผล (เพิ่มใหม่ตามสั่ง)
    // ==========================================
    // วิธีเรียกใช้งานจากโค้ดอื่นเมื่อจบเกม: Sc3_NotificationUI.Instance.ShowSummaryWindow();
    public void ShowSummaryWindow()
    {
        if (summaryWindow == null)
        {
            Debug.LogError("[Summary UI] กรุณาลากหน้าต่างสรุปผลมาใส่ในช่อง Summary Window ก่อนสั่งทำงานครับ!");
            return;
        }

        if (summaryRoutine != null)
        {
            StopCoroutine(summaryRoutine);
        }

        summaryRoutine = StartCoroutine(ZoomInSummarySequence());
    }

    // IEnumerator สำหรับซูมขยายหน้าต่างสรุปผลจากกลางจอ
    private IEnumerator ZoomInSummarySequence()
    {
        // รีเซ็ตให้หดเหลือ 0 ก่อนเริ่มขยาย
        summaryWindow.localScale = Vector3.zero;

        // ค่อยๆ ขยายขนาดจนกว่าจะใกล้เคียงเป้าหมายที่กำหนด (targetScale)
        while (Vector3.Distance(summaryWindow.localScale, targetScale) > 0.001f)
        {
            summaryWindow.localScale = Vector3.Lerp(summaryWindow.localScale, targetScale, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // ล็อกค่าสุดท้ายให้เป๊ะ
        summaryWindow.localScale = targetScale;
        Debug.Log("[Summary UI] หน้าต่างสรุปผลขยายใหญ่เรียบร้อยแล้ว!");
    }
}