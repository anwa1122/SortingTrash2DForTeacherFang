using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Sc3_Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Sc2_TrashData data;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector3 originalPosition;
    private Transform startParent;

    // เก็บสถานะว่ากำลังถูกลากอยู่หรือไม่ เพื่อไม่ให้ค่า alpha ตีกัน
    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        startParent = transform.parent;
        originalPosition = rectTransform.localPosition;
    }

    // --- ส่วนที่เพิ่มเข้ามาใหม่ ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging) // ถ้าไม่ได้ลากอยู่
        {
            canvasGroup.alpha = 0.6f;
            // ย่อขนาดลงเหลือ 90%
            transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging) // ถ้าไม่ได้ลากอยู่
        {
            canvasGroup.alpha = 1f;
            // คืนค่าขนาดปกติ
            transform.localScale = Vector3.one;
        }
    }

    // --- ส่วนเดิม ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true; // ล็อกสถานะ

        if (transform.parent != startParent)
        {
            Vector2 localPoint;
            RectTransform startParentRect = startParent.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(startParentRect, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                transform.SetParent(startParent);
                rectTransform.anchoredPosition = localPoint;
            }
        }

        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false; // ปลดล็อกสถานะ
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (transform.parent == startParent)
        {
            rectTransform.localPosition = ClampToSpawnArea(rectTransform.localPosition);
        }
        else
        {
            Vector2 localPoint;
            RectTransform currentParentRect = transform.parent.GetComponent<RectTransform>();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(currentParentRect, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                rectTransform.anchoredPosition = localPoint;
            }
        }
    }

    private Vector3 ClampToSpawnArea(Vector3 targetPos)
    {
        // ... โค้ดเดิมของคุณ (ไม่ต้องแก้ไข)
        if (startParent != null)
        {
            RectTransform areaRect = startParent.GetComponent<RectTransform>();
            if (areaRect != null)
            {
                float paddingX = rectTransform.rect.width / 2f;
                float paddingY = rectTransform.rect.height / 2f;
                float minX = -areaRect.rect.width / 2f + paddingX;
                float maxX = areaRect.rect.width / 2f - paddingX;
                float minY = -areaRect.rect.height / 2f + paddingY;
                float maxY = areaRect.rect.height / 2f - paddingY;
                float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
                float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);
                return new Vector3(clampedX, clampedY, 0);
            }
        }
        return targetPos;
    }
}