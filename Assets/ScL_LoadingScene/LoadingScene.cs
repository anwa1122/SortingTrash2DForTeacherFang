using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text progressText;
    public Image heartImage;
    public Sprite[] heartSprites;

    [Header("Loading Settings")]
    [Tooltip("เวลาขั้นต่ำที่ใช้ในการโหลดฉาก (วินาที) ยิ่งน้อยยิ่งโหลดเสร็จไว")]
    public float minLoadTime = 3f;

    // 🌟 ตัวแปร Static ระดับโลกสำหรับฝากชื่อซีนปลายทางเอาไว้
    // ตั้งค่าเริ่มต้นไว้ที่ฉากเมนูเผื่อกันบั๊กเปิดฉากพลาด
    public static string TargetSceneName = "MainMenuScene";

    // 🌟 ฟังก์ชันทางลัด (Static) สำหรับให้สคริปต์อื่นเรียกใช้เพื่อสั่งเปลี่ยนฉากผ่านสคริปต์นี้ได้ทันที
    public static void LoadSceneWithLoadingScreen(string sceneToOpen)
    {
        TargetSceneName = sceneToOpen;                  // 1. ฝากชื่อซีนที่จะไป
        SceneManager.LoadScene("ScD_Loading");          // 2. เปิดตัวหน้าต่างโหลดซีน (🚨 เช็คชื่อซีนให้ตรงกับใน Build Settings น้า)
    }

    void Start()
    {
        // 🚀 เปลี่ยนจากเดิมที่ฟิกซ์ชื่อตายตัว มาดึงจากตัวแปรสากลที่เราฝากเอาไว้แทนแล้วครับ!
        StartCoroutine(LoadAsync(TargetSceneName));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;
        float elapsed = 0f;

        while (!operation.isDone)
        {
            elapsed += Time.deltaTime; //

            float realProgress = Mathf.Clamp01(operation.progress / 0.9f); //
            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime / minLoadTime); //

            int index = Mathf.Clamp(Mathf.RoundToInt(fakeProgress * 10), 0, 10); //
            if (heartSprites != null && heartSprites.Length > index) //
            {
                heartImage.sprite = heartSprites[index]; //
            }

            progressText.text = "Loading... " + (int)(fakeProgress * 100) + "%"; //

            // 🚨 [แก้ไขจุดนี้]: เมื่อโหลดเสร็จครบถ้วนสมบูรณ์แล้ว
            if (operation.progress >= 0.9f && elapsed >= minLoadTime && fakeProgress >= 1f)
            {
                // ถ้าในฉากโหลดมีสคริปต์ม่านขาววางอยู่ ให้สั่งเล่นแอนิเมชันปิดตาก่อนย้ายซีน!
                if (Sc2_SceneTransition.Instance != null)
                {
                    // 🎬 เล่นแอนิเมชันปิดม่านขาวจนมิดจอในซีนโหลด
                    yield return StartCoroutine(Sc2_SceneTransition.Instance.FadeOutRoutineBeforeExit());
                }

                // ม่านขาวบังมิดแล้ว ปล่อยตัวให้สลับเข้าซีนใหม่ได้เลย!
                operation.allowSceneActivation = true; //
            }

            yield return null; //
        }
    }
}