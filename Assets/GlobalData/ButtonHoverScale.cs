using UnityEngine;
using UnityEngine.EventSystems; // จำเป็นต้องใช้สำหรับดักจับเมาส์ ชี้/ออก
using System.Collections;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("--- Scale Settings (ตั้งค่าการขยาย) ---")]
    [Tooltip("ขนาดของปุ่มตอนที่เมาส์มาชี้ (แนะนำ 1.15 ถึง 1.25)")]
    public float hoverScaleAmount = 1.2f;

    [Tooltip("ความเร็วในการขยายตัวและหดตัว")]
    public float scaleSpeed = 8f;

    [Header("--- Wiggle Settings (ตั้งค่าการส่ายช้าๆ ตอนเมาส์ชี้) ---")]
    [Tooltip("เปิด/ปิด ระบบส่ายตอนเมาส์ชี้")]
    public bool useWiggleEffect = true;

    [Tooltip("ความเร็วในการส่ายไปมา ยิ่งเยอะยิ่งส่ายรัว (แนะนำ 3 - 5 สำหรับส่ายช้าๆ น่ารัก)")]
    public float wiggleSpeed = 2f;

    [Tooltip("องศาความเอียงสูงสุดในการแกว่ง (แนะนำ 3 - 6 องศา จะได้ไม่เวียนหัวครับ)")]
    public float maxWiggleAngle = 4f;

    private Vector3 initialScale;
    private Quaternion initialRotation;
    private Coroutine scaleCoroutine;
    private bool isHovered = false;
    private float randomOffset;

    void Awake()
    {
        // บันทึกขนาดและองศาเริ่มต้นของปุ่มเอาไว้
        initialScale = transform.localScale;
        initialRotation = transform.localRotation;

        // สุ่มตัวเลขเริ่มต้นเล็กน้อย เพื่อให้ถ้ามีหลายปุ่ม จังหวะการส่ายจะได้ไม่พร้อมกันจนดูเป็นหุ่นยนต์
        randomOffset = Random.Range(0f, 100f);
    }

    // 🎯 เมาส์เล็ง/ชี้เข้ามาที่ปุ่ม
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StopActiveCoroutine();
        scaleCoroutine = StartCoroutine(ScaleRoutine(initialScale * hoverScaleAmount));
    }

    // 🎯 เมาส์เลื่อนออกจากปุ่ม
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        StopActiveCoroutine();
        scaleCoroutine = StartCoroutine(ScaleRoutine(initialScale));
    }

    void Update()
    {
        // 🔄 ถ้าเมาส์ชี้อยู่ และเปิดใช้งานระบบส่าย ให้คำนวณการแกว่งสลับซ้ายขวาใน Update
        if (useWiggleEffect && isHovered)
        {
            float time = Time.time * wiggleSpeed + randomOffset;
            float zRotation = Mathf.Sin(time) * maxWiggleAngle;
            transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
        }
    }

    private void StopActiveCoroutine()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
    }

    // 🎬 คอร์รูทีนค่อยๆ ยืดหดขนาด
    IEnumerator ScaleRoutine(Vector3 targetScale)
    {
        float progress = 0f;
        Vector3 currentScale = transform.localScale;
        Quaternion currentRotation = transform.localRotation;

        while (progress < 1f)
        {
            progress += Time.deltaTime * scaleSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, progress);

            // ขยายขนาดแบบสมูท
            transform.localScale = Vector3.Lerp(currentScale, targetScale, smoothT);

            // 🚨 ถ้าเมาส์ออกจากปุ่ม ให้ค่อยๆ ดึงองศากลับมาตั้งตรงนิ่งๆ แบบนุ่มนวลด้วย
            if (!isHovered)
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, initialRotation, smoothT);
            }

            yield return null;
        }

        transform.localScale = targetScale;

        if (!isHovered)
        {
            transform.localRotation = initialRotation;
        }
    }

    void OnDisable()
    {
        isHovered = false;
        StopActiveCoroutine();
        transform.localScale = initialScale;
        transform.localRotation = initialRotation;
    }
}