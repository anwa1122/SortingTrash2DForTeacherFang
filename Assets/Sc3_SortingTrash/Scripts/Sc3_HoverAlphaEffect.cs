using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Sc3_HoverAlphaEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Alpha Settings")]
    public float normalAlpha = 1f;  // อัลฟ่าปกติ
    public float hoverAlpha = 0.6f; // อัลฟ่าตอนเมาส์เล็ง + กำลังมีการลากอยู่

    private CanvasGroup canvasGroup;
    private bool isPointerOver = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        canvasGroup.alpha = normalAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        // มืดลงเฉพาะตอน "เมาส์เล็งตัวนี้" และ "มีการลากเกิดขึ้นจาก Sc3_Dragable"
        if (isPointerOver && Sc3_Dragable.IsDragging)
        {
            canvasGroup.alpha = hoverAlpha;
        }
        else
        {
            canvasGroup.alpha = normalAlpha;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        // ถ้าแค่เล็งเฉยๆ ไม่ได้กำลังลาก จะไม่ทำอะไร (จัดการใน Update)
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }
}