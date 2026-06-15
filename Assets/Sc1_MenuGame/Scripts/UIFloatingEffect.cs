using UnityEngine;

public class UIFloatingEffect : MonoBehaviour
{
    [Header("การตั้งค่าการเคลื่อนที่")]
    [Tooltip("ระยะห่างที่เลื่อนขึ้นลง (หน่วยเป็นพิกเซล)")]
    public float floatAmount = 50f;

    [Tooltip("ความเร็วในการเลื่อน")]
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        // เก็บตำแหน่งเริ่มต้นเอาไว้
        startPos = transform.position;
    }

    void Update()
    {
        // ใช้ Sin Wave ในการคำนวณตำแหน่งขึ้นลง
        float newY = startPos.y + (Mathf.Sin(Time.time * speed) * floatAmount);

        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}