using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text progressText;
    public Image heartImage;        // สำหรับใส่รูปหัวใจ UI
    public Sprite[] heartSprites;   // สำหรับใส่รูปหัวใจ 11 รูป (0% - 100%)

    [Header("Loading Settings")]
    [Tooltip("เวลาขั้นต่ำที่ใช้ในการโหลดฉาก (วินาที) ยิ่งน้อยยิ่งโหลดเสร็จไว")]
    public float minLoadTime = 3f;  // ← สามารถปรับเปลี่ยนค่าเริ่มต้นตรงนี้ หรือไปปรับใน Unity Inspector ก็ได้

    void Start()
    {
        StartCoroutine(LoadAsync("Sc2_GettingTrash"));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;
        float elapsed = 0f;

        while (!operation.isDone)
        {
            elapsed += Time.deltaTime;

            // คำนวณความคืบหน้าจริงของการโหลด (Unity จะหยุดที่ 0.9 เมื่อโหลดเสร็จแต่ยังไม่ได้เปิดฉาก)
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // ให้เปอร์เซ็นต์ค่อยๆ วิ่งตามความเร็วที่กำหนดใน minLoadTime
            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime / minLoadTime);

            // เปลี่ยนรูปหัวใจตามช่วงเปอร์เซ็นต์ (0 - 10)
            int index = Mathf.Clamp(Mathf.RoundToInt(fakeProgress * 10), 0, 10);
            if (heartSprites != null && heartSprites.Length > index)
            {
                heartImage.sprite = heartSprites[index];
            }

            // แสดงตัวเลขเปอร์เซ็นต์ 0% - 100%
            progressText.text = "Loading... " + (int)(fakeProgress * 100) + "%";

            // ถ้าโหลดตัวฉากจริงเสร็จแล้ว (>= 0.9) และเวลาจำลองผ่านไปจนครบกำหนดแล้ว
            if (operation.progress >= 0.9f && elapsed >= minLoadTime && fakeProgress >= 1f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}