using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Sc2_SceneTransition : MonoBehaviour
{
    public static Sc2_SceneTransition Instance { get; private set; }

    [Header("--- UI Elements ---")]
    public Canvas transitionCanvas; //
    public Image transitionImage; //

    [Header("--- Transition Settings ---")]
    public float transitionSpeed = 2f; //
    public float maxScaleAmount = 1.5f; //

    private RectTransform imgRect;
    private bool isTransitioning = false; //

    private Vector2 centerPos = Vector2.zero; //
    private Vector2 rightPos; //
    private Vector2 leftPos; //

    void Awake()
    {
        Instance = this; //

        if (transitionImage != null)
        {
            imgRect = transitionImage.GetComponent<RectTransform>(); //
        }

        CalculateScreenPositions(); //
    }

    void Start()
    {
        // 🎬 ทุกครั้งที่ซีนไหนก็ตามเปิดตัวขึ้นมา (รวมถึงซีนโหลด) ม่านขาวจะสไลด์หนีไปทางซ้ายทันที!
        StartCoroutine(FadeInRoutine()); //
    }

    void CalculateScreenPositions()
    {
        float width = Screen.width; //
        rightPos = new Vector2(width * 1.5f, 0f); //
        leftPos = new Vector2(-width * 1.5f, 0f); //
    }

    public void ChangeScene(string targetSceneName)
    {
        if (isTransitioning) return; //
        StartCoroutine(FadeOutAndLoadRoutine(targetSceneName)); //
    }

    // 🎬 แอนิเมชันเปิดม่าน (กลางจอ -> ซ้าย)
    IEnumerator FadeInRoutine()
    {
        isTransitioning = true; //
        if (transitionCanvas != null) transitionCanvas.enabled = true; //

        float progress = 0f; //

        if (imgRect != null)
        {
            imgRect.anchoredPosition = centerPos; //
            imgRect.localScale = new Vector3(maxScaleAmount, 1f, 1f); //
        }
        if (transitionImage != null) transitionImage.color = Color.white; //

        while (progress < 1f)
        {
            progress += Time.deltaTime * transitionSpeed; //
            float smoothT = Mathf.SmoothStep(0f, 1f, progress); //

            if (imgRect != null) imgRect.anchoredPosition = Vector2.Lerp(centerPos, leftPos, smoothT); //
            if (transitionImage != null) transitionImage.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), smoothT); //

            yield return null; //
        }

        if (transitionCanvas != null) transitionCanvas.enabled = false; //
        isTransitioning = false; //
    }

    // 🎬 แอนิเมชันปิดม่านของซีนเกมปกติ (ขวา -> กลางจอ) แล้วส่งตัวเข้าซีนโหลด
    IEnumerator FadeOutAndLoadRoutine(string targetSceneName)
    {
        yield return StartCoroutine(FadeOutRoutineBeforeExit());

        // 🚀 ส่งตัวเข้าหน้าฉากโหลด LoadingScreen ตัวกลาง
        LoadingScreen.LoadSceneWithLoadingScreen(targetSceneName); //
    }

    // 🎬 ฟังก์ชันกองกลางสำหรับทำแอนิเมชันปิดตา (สไลด์ม่านขาวจากขวาเข้ามาบังตรงกลางจอจนมิดชิด)
    public IEnumerator FadeOutRoutineBeforeExit()
    {
        isTransitioning = true;
        if (transitionCanvas != null) transitionCanvas.enabled = true;

        float progress = 0f;

        if (imgRect != null)
        {
            imgRect.anchoredPosition = rightPos;
            imgRect.localScale = new Vector3(maxScaleAmount, 1f, 1f);
        }
        if (transitionImage != null) transitionImage.color = new Color(1f, 1f, 1f, 0f);

        while (progress < 1f)
        {
            progress += Time.deltaTime * transitionSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, progress);

            if (imgRect != null) imgRect.anchoredPosition = Vector2.Lerp(rightPos, centerPos, smoothT);
            if (transitionImage != null) transitionImage.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, smoothT);

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        isTransitioning = false;
    }
}