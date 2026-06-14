using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Sc3_Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Sc2_TrashData data;

    [Header("Alpha Settings")]
    public float normalAlpha = 1f;   // อัลฟ่าปกติ
    public float hoverAlpha = 0.8f;  // อัลฟ่าตอนเมาส์เล็งเฉยๆ (ไม่ได้ลาก)
    public float dragAlpha = 0.6f;   // อัลฟ่าตอนกำลังลาก

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    // อ้างอิงสคริปต์ลอย (ถ้าไม่มีติดอยู่บน object นี้ก็จะเป็น null และข้ามการทำงานส่วนนี้ไป)
    private Sc3_UIFloatingEffect floatingEffect;

    private Vector3 originalPosition;
    private Transform startParent;

    private bool isDragging = false;
    private bool isPointerOver = false; // เก็บสถานะว่าเมาส์อยู่บนวัตถุไหม

    // --- Flag กลาง บอกว่ามีการลากเกิดขึ้นอยู่หรือไม่ (ใช้ให้สคริปต์อื่นเช็คได้) ---
    public static bool IsDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        floatingEffect = GetComponent<Sc3_UIFloatingEffect>();
    }

    private void Start()
    {
        startParent = transform.parent;
        originalPosition = rectTransform.localPosition;
        canvasGroup.alpha = normalAlpha;
    }

    // --- ส่วนเพิ่ม: เช็คเมาส์เล็ง ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;

        // ถ้าแค่เล็งเฉยๆ (ไม่ได้กำลังลากตัวนี้) ให้มืดลง
        if (!isDragging)
        {
            canvasGroup.alpha = hoverAlpha;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;

        // เลิกเล็งแล้ว และไม่ได้กำลังลาก ให้คืนค่าอัลฟ่าปกติ
        if (!isDragging)
        {
            canvasGroup.alpha = normalAlpha;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true; // เริ่มลาก
        IsDragging = true; // บอกทุกสคริปต์ว่ามีการลากเกิดขึ้น

        // หยุดเอฟเฟกต์ลอยชั่วคราว ไม่ให้แย่งเซ็ตตำแหน่งระหว่างลาก
        // (ไม่งั้นตัว object จะลอยเหลื่อมไม่ตรงกับตำแหน่งเมาส์)
        if (floatingEffect != null)
        {
            floatingEffect.PauseFloating();
        }

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
        // มืดลงตอนลาก
        canvasGroup.alpha = dragAlpha;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false; // จบการลาก
        IsDragging = false; // แจ้งทุกสคริปต์ว่าหยุดลากแล้ว

        canvasGroup.blocksRaycasts = true;

        // คืนค่า Alpha: ถ้าเมาส์ยังเล็งอยู่ ให้เป็น hoverAlpha ไม่งั้นปกติ
        canvasGroup.alpha = isPointerOver ? hoverAlpha : normalAlpha;

        if (transform.parent == startParent)
        {
            rectTransform.localPosition = ClampToSpawnArea(rectTransform.localPosition);

            // ยังอยู่ในพื้นที่เกิด -> ให้ลอยต่อจากตำแหน่งที่ปล่อยจริง (ไม่เด้งกลับจุดเกิดเดิม)
            if (floatingEffect != null)
            {
                floatingEffect.ResumeFloating(rectTransform.anchoredPosition);
            }
        }
        else
        {
            Vector2 localPoint;
            RectTransform currentParentRect = transform.parent.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(currentParentRect, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                rectTransform.anchoredPosition = localPoint;
            }

            // ถูกวางลง slot แล้ว -> หยุดลอย ให้อยู่นิ่งตรงนั้น
            if (floatingEffect != null)
            {
                floatingEffect.PauseFloating();
            }
        }
    }

    private Vector3 ClampToSpawnArea(Vector3 targetPos)
    {
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
                return new Vector3(Mathf.Clamp(targetPos.x, minX, maxX), Mathf.Clamp(targetPos.y, minY, maxY), 0);
            }
        }
        return targetPos;
    }
}