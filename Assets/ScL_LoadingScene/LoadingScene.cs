using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public TMP_Text progressText;
    public Image heartImage;        // ← แทน Slider
    public Sprite[] heartSprites;   // ← ใส่รูป 11 รูป (0%-100%)

    void Start()
    {
        StartCoroutine(LoadAsync("Sc2_GettingTrash"));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;
        float minLoadTime = 3f;
        float elapsed = 0f;

        while (!operation.isDone)
        {
            elapsed += Time.deltaTime;

            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime / minLoadTime);

            // เปลี่ยนรูปหัวใจตาม %
            int index = Mathf.Clamp(Mathf.RoundToInt(fakeProgress * 10), 0, 10);
            heartImage.sprite = heartSprites[index];

            progressText.text = "Loading... " + (int)(fakeProgress * 100) + "%";

            if (operation.progress >= 0.9f && elapsed >= minLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}