using UnityEngine;
using UnityEngine.EventSystems;

public class Sc3_TrashSlot : MonoBehaviour, IDropHandler
{
    [Header("กำหนดประเภทของถังขยะใบนี้")]
    public TrashType slotType;

    public void OnDrop(PointerEventData eventData)
    {
        // ตรวจสอบว่าสิ่งที่เอามาปล่อย มีสคริปต์ลากวาง (Sc3_Dragable) อยู่หรือไม่
        if (eventData.pointerDrag != null)
        {
            Sc3_Dragable dragableItem = eventData.pointerDrag.GetComponent<Sc3_Dragable>();

            if (dragableItem != null)
            {
                // ย้ายตัวขยะมาเป็นลูกของถังขยะใบนี้ (จัดระเบียบใน Hierarchy และดูดเข้าถัง)
                dragableItem.transform.SetParent(transform);

                // ปรับตำแหน่งให้ขยะไปอยู่ตรงกลางถังพอดี
                RectTransform itemRect = dragableItem.GetComponent<RectTransform>();
                itemRect.anchoredPosition = Vector2.zero;
            }
        }
    }
}