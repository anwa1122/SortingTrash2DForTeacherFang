using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Sc2_SceneTransition : MonoBehaviour
{
    // 🌟 ระบบ Instance (Singleton) เรียกใช้งานง่ายจากทุกสคริปต์ในโลก
    public static Sc2_SceneTransition Instance { get; private set; }

    [Header("--- UI Elements ---")]
    [Tooltip("ลาก Canvas ที่ครอบตัว Image สีขาวมาใส่ช่องนี้")]
    public Canvas transitionCanvas;

    [Tooltip("ลาก UI Image สีขาวที่ใช้ทำ Transition มาใส่ช่องนี้")]
    public Image transitionImage;

    [Header("--- Transition Settings ---")]
    [Tooltip("ความเร็วในการเล่นแอนิเมชัน ยิ่งเยอะยิ่งไว (แนะนำ 1.5 - 2.5)")]
    public float transitionSpeed = 2f;

    [Tooltip("ขนาดขยายของแผ่นขาวตอนพุ่งเข้า-ออกเพื่อให้บังมิดชัวร์ๆ (แนะนำ 2 ถึง 3 เท่า)")]
    public float maxScaleAmount = 2.5f;

    private RectTransform imgRect;
    private bool isTransitioning = false;

    // 📐 พิกัดตําแหน่งสําหรับคํานวณการพุ่งเฉียง (อ้างอิงจากขนาดหน้าจอจริง)
    private Vector2 centerPos = Vector2.zero;
    private Vector2 topRightPos;
    private Vector2 bottomLeftPos;

    void Awake()
    {
        // 🔒 ตรวจสอบและสถาปนาตัวเองเป็นตัวกลางเดี่ยวประจำโปรเจกต์
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // บังคับให้วัตถุนี้ไม่โดนทำลายตอนย้ายซีน!
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (transitionImage != null)
        {
            imgRect = transitionImage.GetComponent<RectTransform>();
        }
    }

    void Start()
    {
        CalculateScreenPositions();
        // เข้าเกมมานัดแรก สั่งให้ม่านขาวพุ่งหนีจากกลางจอลงไปซ้ายล่างทันทีเพื่อเผยภาพเกม!
        StartCoroutine(FadeInRoutine());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CalculateScreenPositions();
        // พอย้ายซีนเสร็จปุ๊บ สั่งม่านขาวพุ่งหนีจากกลางจอเคลียร์ทางลงไปซ้ายล่างทันทีอัตโนมัติ!
        StartCoroutine(FadeInRoutine());
    }

    // 🧮 ฟังก์ชันคำนวณพิกัดขอบจอแบบ Dynamic ป้องกันปัญหาจอแต่ละเครื่องขนาดไม่เท่ากัน
    void CalculateScreenPositions()
    {
        float width = Screen.width;
        float height = Screen.height;

        // ดักเผื่อกรณีเป็น Canvas แบบ Scale เติมจอ ให้บวกระยะเผื่อไปเลยเยอะๆ จะได้ไม่เห็นขอบตัด
        topRightPos = new Vector2(width * 1.5f, height * 1.5f);
        bottomLeftPos = new Vector2(-width * 1.5f, -height * 1.5f);
    }

    /// <summary>
    /// 🔥 สั่งย้ายซีนพร้อมคัตซีนพุ่งเฉียงขวาบน-ซ้ายล่างสุดเท่
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        if (isTransitioning) return; // กันบั๊กกดรัวๆ
        StartCoroutine(FadeOutAndLoadRoutine(sceneName));
    }

    // 🎬 แอนิเมชันขาเข้าฉาก (เปิดตา): แผ่นขาวขยายใหญ่จากกลางจอ พุ่งไหลฟุ่บลงไปทาง "ซ้ายล่าง" แล้วจางหายไป
    IEnumerator FadeInRoutine()
    {
        isTransitioning = true;
        if (transitionCanvas != null) transitionCanvas.enabled = true;

        float progress = 0f;

        // สภาพเริ่มต้นขาเข้า: แผ่นขาวบังเต็มจออยู่ตรงกลาง และขยายใหญ่สุดๆ
        if (imgRect != null)
        {
            imgRect.anchoredPosition = centerPos;
            imgRect.localScale = new Vector3(maxScaleAmount, maxScaleAmount, 1f);
        }
        if (transitionImage != null) transitionImage.color = Color.white;

        // 💨 แอนิเมชันรันพุ่งหนีลงซ้ายล่าง + หดขนาดลงเล็กน้อย + ค่อยๆ โปร่งแสง
        while (progress < 1f)
        {
            progress += Time.deltaTime * transitionSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, progress);

            if (imgRect != null)
            {
                // พุ่งทะยานจากจุดกึ่งกลาง (Center) ➡️ ไหลลงไปยังซ้ายล่าง (Bottom Left) พร้อมหดกลับขนาดปกติ
                imgRect.anchoredPosition = Vector2.Lerp(centerPos, bottomLeftPos, smoothT);
                imgRect.localScale = Vector3.Lerp(new Vector3(maxScaleAmount, maxScaleAmount, 1f), Vector3.one, smoothT);
            }
            if (transitionImage != null)
            {
                transitionImage.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), smoothT);
            }

            yield return null;
        }

        if (transitionCanvas != null) transitionCanvas.enabled = false;
        isTransitioning = false;
    }

    // 🎬 แอนิเมชันขาก่อนวาร์ป (ปิดตา): แผ่นขาวพุ่งมาจาก "ขวาบน" พร้อมขยายใหญ่ยักษ์เข้ามากลืนกินหน้าจอมิดชิด
    IEnumerator FadeOutAndLoadRoutine(string sceneName)
    {
        isTransitioning = true;
        if (transitionCanvas != null) transitionCanvas.enabled = true;

        float progress = 0f;

        // สภาพเริ่มต้นขาก่อนวาร์ป: แผ่นขาวแอบซ่อนอยู่ไกลๆ ที่มุมขวาบนนอกสายตา และตัวเล็กปกติ
        if (imgRect != null)
        {
            imgRect.anchoredPosition = topRightPos;
            imgRect.localScale = Vector3.one;
        }
        if (transitionImage != null) transitionImage.color = new Color(1f, 1f, 1f, 0f);

        // 💥 แอนิเมชันรันพุ่งกระแทกเข้าหาตรงกลางจอ + ขยายขนาดใหญ่ยักษ์ทับจอ + สีขาวเข้มขึ้นจนบังมิด
        while (progress < 1f)
        {
            progress += Time.deltaTime * transitionSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, progress);

            if (imgRect != null)
            {
                // พุ่งทะลวงจากขวาบน (Top Right) ➡️ วิ่งเข้ามากระแทกจุดศูนย์กลางจอ (Center) พร้อมขยายขนาดยักษ์ทับจอ
                imgRect.anchoredPosition = Vector2.Lerp(topRightPos, centerPos, smoothT);
                imgRect.localScale = Vector3.Lerp(Vector3.one, new Vector3(maxScaleAmount, maxScaleAmount, 1f), smoothT);
            }
            if (transitionImage != null)
            {
                transitionImage.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, smoothT);
            }

            yield return null;
        }

        // แช่หน้าจอขาวนิ่งเพิ่มอารมณ์ความนุ่มนวลแป๊บนึง
        yield return new WaitForSeconds(0.1f);

        // 🚀 ม่านขาวบังมิดจอแล้ว สั่งย้ายซีนจริงทันที!
        SceneManager.LoadScene(sceneName);
    }
}