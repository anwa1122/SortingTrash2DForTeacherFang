using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text progressText;

    void Start()
    {
        StartCoroutine(LoadAsync("Sc2_GettingTrash"));
    }

IEnumerator LoadAsync(string sceneName)
{
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
    operation.allowSceneActivation = false;

    float fakeProgress = 0f;        // ← เพิ่มตัวแปรนี้
    float minLoadTime = 3f;         // ← กำหนดเวลาขั้นต่ำ (วินาที)
    float elapsed = 0f;             // ← จับเวลาที่ผ่านไป

    while (!operation.isDone)
    {
        elapsed += Time.deltaTime;

        // ค่อยๆ เพิ่ม progress ตามเวลา
        float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
        fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime / minLoadTime);

        progressBar.value = fakeProgress;
        progressText.text = "Loading... " + (int)(fakeProgress * 100) + "%";

        // เปิด Scene เมื่อครบเวลาขั้นต่ำ และโหลดเสร็จแล้ว
        if (operation.progress >= 0.9f && elapsed >= minLoadTime)
        {
            operation.allowSceneActivation = true;
        }

        yield return null;
    }
}
}