using UnityEngine;
using TMPro;
using System.Collections;

public class Sc2_CountdownTimer : MonoBehaviour
{
    [Header("--- Timer Settings ---")]
    [Tooltip("เวลาเริ่มต้นเล่นเกม (หน่วยเป็นวินาที) ปรับได้ตามใจชอบเลยครับ")]
    public float totalTime = 60f;

    [Header("--- UI Elements ---")]
    [Tooltip("ลาก TMP_Text ที่ใช้โชว์ตัวเลขเวลามาใส่ช่องนี้")]
    public TMP_Text timerText;

    [Tooltip("ลาก GameObject ของข้อความ Time Up! (ตัวที่มีสคริปต์ดุ๊กดิ๊กแปะอยู่) มาใส่ช่องนี้")]
    public GameObject timeUpNotification;

    [Header("--- Scene Transition ---")]
    [Tooltip("เวลาที่จะให้โชว์ข้อความ Time Up! ค้างไว้ดุ๊กดิ๊กขู่ผู้เล่น ก่อนจะกดเปลี่ยนฉาก (วินาที)")]
    public float delayBeforeTeleport = 3f;

    private float currentTime;
    private bool isTimeOut = false;

    void Start()
    {
        currentTime = totalTime;

        // เริ่มเกมมาให้ปิดข้อความแจ้งเตือน Time Up! ซ่อนไว้ก่อน
        if (timeUpNotification != null)
        {
            timeUpNotification.SetActive(false);
        }

        UpdateTimerUI();
    }

    void Update()
    {
        // ถ้าเวลาหมดไปแล้ว ไม่ต้องทำลอจิกข้างล่างต่อ
        if (isTimeOut) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // ป้องกันไม่ให้ตัวเลขติดลบ
            if (currentTime <= 0)
            {
                currentTime = 0;
                TimeOutTrigger(); // เรียกฟังก์ชันเวลาหมด
            }

            UpdateTimerUI();
        }
    }

    // ฟังก์ชันจัดฟอร์แมตแปลงวินาทีไปเป็นนาที:วินาที (เช่น 01:30) โชว์บนหน้าจอ
    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime % 60F);

        // แสดงผลในรูปแบบ นาที:วินาที 
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // ทริคเพิ่มเติม: ถ้าเหลือเวลาน้อยกว่า 10 วินาที เปลี่ยนสีตัวอักษรเป็นสีแดงให้ตื่นเต้นเร้าใจขึ้น
        if (currentTime <= 10f)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    // ฟังก์ชันทำงานทันทีเมื่อเวลาหมดลง
    void TimeOutTrigger()
    {
        isTimeOut = true;

        // 1. เปิดแสดงผลข้อความแจ้งเตือน Time Up! ทันที
        if (timeUpNotification != null)
        {
            timeUpNotification.SetActive(true);
        }

        // 2. รันคำสั่ง Coroutine สั่งหน่วงเวลาโชว์ความน่ารักของตัวอักษรก่อนวาร์ป
        StartCoroutine(WaitAndTeleport());
    }

    IEnumerator WaitAndTeleport()
    {
        // นั่งรอชิลๆ ให้ตัวอักษรส่ายไปมาตามเวลาที่เราตั้งไว้
        yield return new WaitForSeconds(delayBeforeTeleport);

        // 3. วาร์ปไปยัง Scene 3 โดยผ่าน LoadingScreen ตัวกลางรูปหัวใจของเราเรียบร้อยครับ!
        LoadingScreen.LoadSceneWithLoadingScreen("Sc3_SortingTrash");
    }
}