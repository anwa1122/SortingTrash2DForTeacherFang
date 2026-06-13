using UnityEngine;
using TMPro;
using System.Collections;

public class Sc2_CountdownTimer : MonoBehaviour
{
    [Header("--- Timer Settings (ตั้งค่าเวลาเล่นเกมจริง) ---")]
    [Tooltip("เวลาเล่นเกมในฉากนี้จริง (หน่วยเป็นวินาที)")]
    public float totalTime = 60f;

    [Header("--- UI Elements ---")]
    [Tooltip("ลาก TMP_Text ที่ใช้โชว์ตัวเลขเวลานับถอยหลังหลักด้านบนมาใส่ (TimerText)")]
    public TMP_Text timerText;

    [Tooltip("ลาก GameObject ของข้อความ Time Up! แยกเฉพาะมาใส่ช่องนี้ (TimeUpPanel)")]
    public GameObject timeUpNotification;

    [Header("--- 🎮 New Order Setup (ระบบแยกป้ายทำตามสั่ง) ---")]
    [Tooltip("🔥 ลาก GameObject ป้ายบอกภารกิจมาใส่ช่องนี้ (MissionText)")]
    public GameObject missionTextObject;

    [Tooltip("🔥 ลาก GameObject ป้ายตัวเลขนับถอยหลัง 3 2 1 Go มาใส่ช่องนี้ (MissionNotification)")]
    public GameObject introCountObject;

    [Header("--- Scene Transition ---")]
    [Tooltip("เวลาที่จะให้โชว์ข้อความ Time Up! ค้างไว้ก่อนวาร์ป (วินาที)")]
    public float delayBeforeTeleport = 3f;

    // 🌟 ตัวแปรสากล (Global Switch) สำหรับหยุดเดินผู้เล่นชั่วคราว
    public static bool IsIntroCounting { get; private set; }

    private float currentTime;
    private bool isTimeOut = false;
    private bool isGameStarted = false;

    private Sc2_UINotifWiggle missionTextWiggle;
    private Sc2_UINotifWiggle introCountWiggle;
    private Sc2_UINotifWiggle timeUpWiggle;
    private TMP_Text introTMPText;

    void Awake()
    {
        // 🔒 ล็อกขาผู้เล่นทันทีตั้งแต่เสี้ยววินาทีแรกที่เกมโหลดฉากนี้ขึ้นมา!
        IsIntroCounting = true;
    }

    void Start()
    {
        currentTime = totalTime;

        // ค้นหาดึงสคริปต์และคอมโพเนนต์มาเก็บไว้สั่งงานตรงๆ
        if (missionTextObject != null) missionTextWiggle = missionTextObject.GetComponent<Sc2_UINotifWiggle>();

        if (introCountObject != null)
        {
            introCountWiggle = introCountObject.GetComponent<Sc2_UINotifWiggle>();
            introTMPText = introCountObject.GetComponent<TMP_Text>(); // ดึงตัวหนังสือในร่างเดียวมาใช้งาน
        }

        if (timeUpNotification != null) timeUpWiggle = timeUpNotification.GetComponent<Sc2_UINotifWiggle>();

        // 🛑 เคลียร์สถานะปิดตา UI ทุกชิ้นในเฟรมแรก ป้องกันปัญหาโผล่มาทับซ้อนกันมั่ว
        if (missionTextObject != null) missionTextObject.SetActive(false);
        if (introCountObject != null) introCountObject.SetActive(false);
        if (timeUpNotification != null) timeUpNotification.SetActive(false);

        UpdateTimerUI();

        // 🎬 เริ่มต้นการรันลำดับเหตุการณ์ (Flow) ตามที่คุณกำหนด
        StartCoroutine(GameIntroFlowRoutine());
    }

    void Update()
    {
        // ถ้าช่วงแนะนำกติกา/นับเลขยังไม่จบ หรือเวลาหมดแมพแล้ว ห้ามหักเวลาด้านบนเด็ดขาด
        if (!isGameStarted || isTimeOut) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                TimeOutTrigger();
            }

            UpdateTimerUI();
        }
    }

    IEnumerator GameIntroFlowRoutine()
    {
        IsIntroCounting = true; // ย้ำสถานะล็อกขา

        // 🎬 จังหวะที่ 1: โชว์ป้ายคำสั่งภารกิจ (MissionText) เพียวๆ แช่ทิ้งไว้ 3 วินาทีตามสั่ง โดยที่เวลาหลักยังไม่นับ
        if (missionTextObject != null)
        {
            missionTextObject.SetActive(true);
            if (missionTextWiggle != null) missionTextWiggle.StartSlideUpEffect(); // ดีดตัวสไลด์ขึ้นมาโชว์
            yield return new WaitForSeconds(3.0f); // บังคับแช่ค้าง 3 วินาทีถ้วน
            missionTextObject.SetActive(false); // ครบเวลาสั่งปิดป้ายภารกิจเคลียร์ทาง
        }

        // 🎬 จังหวะที่ 2: เริ่มต้นนับถอยหลังผ่านป้ายตัวเลข (MissionNotification) ทุบกระแทกหน้าจอทีละนัด
        if (introCountObject != null && introTMPText != null)
        {
            introCountObject.SetActive(true);

            // 💥 ทุบเลข 3!
            introTMPText.text = "3";
            if (introCountWiggle != null) introCountWiggle.StartSlamZoomEffect();
            yield return new WaitForSeconds(1.0f);

            // 💥 ทุบเลข 2!
            introTMPText.text = "2";
            if (introCountWiggle != null) introCountWiggle.StartSlamZoomEffect();
            yield return new WaitForSeconds(1.0f);

            // 💥 ทุบเลข 1!
            introTMPText.text = "1";
            if (introCountWiggle != null) introCountWiggle.StartSlamZoomEffect();
            yield return new WaitForSeconds(1.0f);

            // 💥 ทุบคำว่า GO!!!
            introTMPText.text = "GO!!!";
            if (introCountWiggle != null) introCountWiggle.StartSlamZoomEffect();

            // 🎉 [จุดเริ่มเกมจริง!] คลายล็อกขาผู้เล่นให้วิ่งได้ทันที และเปิดให้เวลาหลักเริ่มวิ่งถอยหลังพร้อมกันเป๊ะ!
            IsIntroCounting = false;
            isGameStarted = true;

            yield return new WaitForSeconds(1.0f);
            introCountObject.SetActive(false); // เคลียร์ป้ายนับออก หน้าจอโล่งพร้อมเล่นเกม
        }
        else
        {
            // ระบบสำรองกันเกมค้างกรณีผูกสายสัมพันธ์พลาด
            IsIntroCounting = false;
            isGameStarted = true;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 10f) timerText.color = Color.red;
        else timerText.color = Color.white;
    }

    void TimeOutTrigger()
    {
        isTimeOut = true;
        IsIntroCounting = true; // เวลาหมด สั่งล็อกขาผู้เล่นนิ่งสนิททันทีเดินต่อไม่ได้

        // 💥 เรียกเปิดเฉพาะป้ายแจ้งเตือน Time Up! ออกมากระแทกหน้าจอดุ๊กดิ๊ก
        if (timeUpNotification != null)
        {
            timeUpNotification.SetActive(true);
            if (timeUpWiggle != null) timeUpWiggle.StartSlamZoomEffect();
        }

        StartCoroutine(WaitAndTeleport());
    }

    IEnumerator WaitAndTeleport()
    {
        yield return new WaitForSeconds(delayBeforeTeleport);
        // สั่งผ่านระบบใหม่ ม่านขาวจะสไลด์ปิดตาก่อนวาร์ปทันที นุ่มนวลน่ารักชัวร์!
        Sc2_SceneTransition.Instance.ChangeScene("Sc3_SortingTrash");
        //LoadingScreen.LoadSceneWithLoadingScreen("Sc3_SortingTrash");
    }
}