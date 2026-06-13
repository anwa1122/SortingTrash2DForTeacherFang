using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Sc2_SceneTransition : MonoBehaviour
{
    // 🌟 ระบบ Instance (Singleton) เรียกใช้งานง่ายจากทุกสคริปต์ในโลกe
    public static Sc2_SceneTransition Instance { get; private set; }

    [Header("--- UI Elements ---")]
    [Tooltip("ลาก Canvas ที่ครอบตัว Image สีขาวมาใส่ช่องนี้")]
    public Canvas transitionCanvas;

    [Tooltip("ลาก UI Image สีขาวที่ใช้ทำ Transition มาใส่ช่องนี้")]
    public Image transitionImage;

    [Header("--- Transition Settings ---")]
    [Tooltip("ความเร็วในการเล่นแอนิเมชัน ยิ่งเยอะยิ่งไว (แนะนำ 1.5 - 2.5)")]
    public float transitionSpeed = 2f;

    private RectTransform imgRect;
    private bool isTransitioning = false;

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
        // ทุกครั้งที่เปิดเกมหรือโหลดตัวมันขึ้นมา ให้เริ่มฉากเปิดตัวทันที!
        StartCoroutine(FadeInRoutine());
    }

    // 🔄 ฟังก์ชันพิเศษของ Unity: สั่งงานทุกครั้งที่ระบบตรวจพบว่า "โหลดซีนเสร็จสิ้นแล้ว"
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
        // พอย้ายซีนเสร็จปุ๊บ สั่งแอนิเมชันเปิดตาเผยโฉมฉากใหม่ทันทีอัตโนมัติ!
        StartCoroutine(FadeInRoutine());
    }

    /// <summary>
    /// 🔥 ฟังก์ชันไม้ตายสำหรับเรียกใช้จากสคริปต์อื่นเพื่อเปลี่ยนซีนพร้อมคัตซีนสุดน่ารัก
    /// ตัวอย่างวิธีใช้: Sc2_SceneTransition.Instance.ChangeScene("Sc3_SortingTrash");
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        if (isTransitioning) return; // กันบั๊กกดรัวๆ
        StartCoroutine(FadeOutAndLoadRoutine(sceneName));
    }

    // 🎬 แอนิเมชันขาเข้าฉาก (เปิดตา): ม่านสีขาวหดตัวหายไปอย่างน่ารักนุ่มนวล
    IEnumerator FadeInRoutine()
    {
        isTransitioning = true;
        if (transitionCanvas != null) transitionCanvas.enabled = true;

        float progress = 0f;

        // แอนิเมชัน: จากจอขาวโพลน ค่อยๆ ยืดและหดจางหายไป (ใช้การยืดหด Scale + จางสีคู่กันเพิ่มความนุ่มนวล)
        while (progress < 1f)
        {
            progress += Time.deltaTime * transitionSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, progress);

            // เอฟเฟกต์ยืดจางน่ารัก: Image จะค่อยๆ ขยายกว้างออกพร้อมกับจางค่า Alpha (โปร่งแสง) ลง
            if (imgRect != null)
            {
                imgRect.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.5f, 1.5f, 1f), smoothT);
            }
            if (transitionImage != null)
            {
                transitionImage.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), smoothT);
            }

            yield return null;
        }

        // เล่นจบสั่งปิดตัว Canvas ทันทีเพื่อไม่ให้ไปบล็อกปุ่มเมาส์ในฉากเกม
        if (transitionCanvas != null) transitionCanvas.enabled = false;
        isTransitioning = false;
    }

    // 🎬 แอนิเมชันขาก่อนวาร์ป (ปิดตา): แผ่นขาวพุ่งกระแทกเข้าตาบดบังทั้งจออย่างนุ่มนวล แล้วค่อยโหลดซีน
    IEnumerator FadeOutAndLoadRoutine(string sceneName)
    {
        isTransitioning = true;
        if (transitionCanvas != null) transitionCanvas.enabled = true;

        float progress = 0f;

        // สภาพเริ่มต้น: ขยายใหญ่และโปร่งแสงอยู่
        if (imgRect != null) imgRect.localScale = new Vector3(1.5f, 1.5f, 1f);
        if (transitionImage != null) transitionImage.color = new Color(1f, 1f, 1f, 0f);

        // แอนิเมชัน: ค่อยๆ ทุบหดขนาดกลับมาเท่าหน้าจอเป๊ะ พร้อมสีขาวเข้มขึ้นจนบังมิด
        while (progress < 1f)
        {
            progress += Time.deltaTime * transitionSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, progress);

            if (imgRect != null)
            {
                imgRect.localScale = Vector3.Lerp(new Vector3(1.5f, 1.5f, 1f), Vector3.one, smoothT);
            }
            if (transitionImage != null)
            {
                transitionImage.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, smoothT);
            }

            yield return null;
        }

        // แช่หน้าจอขาวไว้นิดนึง 0.1 วินาทีเพิ่มความสมูทอารมณ์กล้องชัตเตอร์
        yield return new WaitForSeconds(0.1f);

        // 🚀 สั่งย้ายซีนจริง! (หลังจากม่านขาวบังมิดหน้าจอแล้ว)
        SceneManager.LoadScene(sceneName);
    }
}