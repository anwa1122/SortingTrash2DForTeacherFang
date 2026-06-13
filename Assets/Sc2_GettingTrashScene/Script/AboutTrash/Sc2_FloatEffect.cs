using UnityEngine;

public class Sc2_FloatEffect : MonoBehaviour
{
    [Header("--- ตั้งค่าช่วงการสุ่มความเร็วลอย (Floating Speed Range) ---")]
    [Tooltip("ความเร็วลอยต่ำสุด (ยิ่งน้อยยิ่งช้า)")]
    public float minFloatSpeed = 1.0f;
    [Tooltip("ความเร็วลอยสูงสุด")]
    public float maxFloatSpeed = 2.5f;

    [Header("--- ตั้งค่าช่วงการสุ่มระยะลอย (Floating Amplitude Range) ---")]
    [Tooltip("ระยะทางลอยขึ้นลงสั้นที่สุด (หน่วยเป็นเมตร/ช่อง Unity)")]
    public float minFloatAmplitude = 0.08f;
    [Tooltip("ระยะทางลอยขึ้นลงยาวที่สุด")]
    public float maxFloatAmplitude = 0.18f;

    [Header("--- ตั้งค่าการหมุนส่าย (Rotation Settings) ---")]
    [Tooltip("ถ้าติ๊กถูก วัตถุจะหมุนเอียงไปมาซ้ายขวาเบาๆ")]
    public bool useRotateEffect = true;

    [Tooltip("ความเร็วในการหมุนเอียงต่ำสุด")]
    public float minRotateSpeed = 1.0f;
    [Tooltip("ความเร็วในการหมุนเอียงสูงสุด")]
    public float maxRotateSpeed = 2.2f;

    [Tooltip("องศาที่ยอมให้เอียงมากที่สุด (สุ่มตั้งแต่ 0 ถึงค่านี้)")]
    public float maxRotateAngleLimit = 10f;

    // ตัวแปรสำหรับเก็บค่าฟิสิกส์ที่ถูกสุ่มเลือกเฉพาะตัวของขยะชิ้นนั้นๆ
    private float actualFloatSpeed;
    private float actualFloatAmplitude;
    private float actualRotateSpeed;
    private float actualRotateAngle;

    private Vector3 startPosition;
    private float randomOffset;

    void Start()
    {
        // 1. บันทึกตำแหน่งเริ่มต้นของวัตถุไว้
        startPosition = transform.localPosition;

        // 2. สุ่มจุดเริ่มต้นของเวลา (Time Offset) กันวัตถุขยับพร้อมกัน
        randomOffset = Random.Range(0f, 100f);

        // 3. 🎲 ทำการสุ่มค่าความเร็วและระยะต่างๆ เฉพาะตัวของขยะชิ้นนี้ชิ้นเดียว!
        actualFloatSpeed = Random.Range(minFloatSpeed, maxFloatSpeed);
        actualFloatAmplitude = Random.Range(minFloatAmplitude, maxFloatAmplitude);

        actualRotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
        // สุ่มองศาการเอียงสูงสุดของชิ้นนี้ เช่น บางชิ้นเอียงนิดเดียว (3 องศา) บางชิ้นเอียงเยอะ (10 องศา)
        actualRotateAngle = Random.Range(2f, maxRotateAngleLimit);
    }

    void Update()
    {
        // --- ลอจิกการลอยขึ้น-ลง (Sine Wave) โดยใช้ค่าที่สุ่มมาได้ ---
        float time = Time.time * actualFloatSpeed + randomOffset;
        float newY = startPosition.y + Mathf.Sin(time) * actualFloatAmplitude;

        // อัปเดตตำแหน่งเฉพาะแกน Y ของตัวลูก
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);

        // --- ลอจิกการหมุนเอียงซ้ายขวาเบาๆ โดยใช้ค่าที่สุ่มมาได้ ---
        if (useRotateEffect)
        {
            float rotateTime = Time.time * actualRotateSpeed + randomOffset;
            float zRotation = Mathf.Sin(rotateTime) * actualRotateAngle;

            // อัปเดตการหมุนแกน Z เฉพาะของตัวลูก
            transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
        }
    }
}