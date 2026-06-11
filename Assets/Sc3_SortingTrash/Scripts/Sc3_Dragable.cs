using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Sc3_Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Sc2_TrashData data; // ข้อมูลขยะชิ้นนี้

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    // ตัวแปรสำหรับจำพิกัดและ Parent แรกเริ่ม (ItemFather)
    private Vector3 originalPosition;
    private Transform startParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        // บันทึก Parent แรกเริ่ม (ItemFather) และพิกัดสุ่มตอนเกิดเอาไว้
        startParent = transform.parent;
        originalPosition = rectTransform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 🔥 จังหวะที่เริ่มลาก: ย้ายกลับมาเป็นลูกของ ItemFather ทันที (หลุดจากถังเก่าถ้าเคยใส่ไว้)
        if (transform.parent != startParent)
        {
            // แปลงพิกัดหน้าจอ ณ จุดที่เมาส์จิ้ม ให้กลายเป็นพิกัด Local ของ ItemFather เพื่อไม่ให้วัตถุกระตุกวาร์ป
            Vector2 localPoint;
            RectTransform startParentRect = startParent.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(startParentRect, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                transform.SetParent(startParent);
                rectTransform.anchoredPosition = localPoint;
            }
        }

        // สั่งให้วัตถุนี้อยู่บนสุดของเลเยอร์ใน ItemFather ทันทีตอนกำลังลาก
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
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // เช็คว่าปล่อยเมาส์แล้ว Parent เปลี่ยนไปเป็นของ Slot หรือยัง (ดูว่าสคริปต์ Sc3_TrashSlot ทำงานสำเร็จไหม)
        // บรรทัดนี้จะเช็คว่าหลังจากปล่อยเมาส์ มันยังเป็นลูกของ ItemFather อยู่รึเปล่า
        if (transform.parent == startParent)
        {
            // 1. ถ้าปล่อยนอกถังขยะ (ไม่โดน Slot ไหนเลย) -> ให้เด้งกลับไปที่ขอบพื้นที่เกิด
            rectTransform.localPosition = ClampToSpawnArea(rectTransform.localPosition);
        }
        else
        {
            // 2. ถ้าปล่อยในถังขยะสำเร็จ (สคริปต์ของ Slot ยึดมันไปเป็นลูกแล้ว)
            // ให้คำนวณตำแหน่งปัจจุบันให้อยู่บนพื้นที่ของ Slot นั้นๆ ตามพิกัดเมาส์ที่ปล่อย
            Vector2 localPoint;
            RectTransform currentParentRect = transform.parent.GetComponent<RectTransform>();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(currentParentRect, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                rectTransform.anchoredPosition = localPoint;
            }
        }
    }

    // ฟังก์ชันช่วยบีบพิกัดขยะไม่ให้ลอยหลุดขอบของ ItemFather
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

                float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
                float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

                return new Vector3(clampedX, clampedY, 0);
            }
        }
        return targetPos;
    }
}