using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class Sc3_CompleteTheGame : MonoBehaviour
{
    public static Sc3_CompleteTheGame Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RunCompleteGame(int finalScore)
    {
        Sc3_SummaryManager.Instance.UpdateAndShowSummary(finalScore);
        Sc3_SummaryManager.Instance.UpdateLeaderboard();
        StartCoroutine(WaitAndMoveToScene());
    }

    private IEnumerator WaitAndMoveToScene()
    {
        // ⏳ สั่งให้หยุดรอตรงนี้เป็นเวลา 1 วินาที
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("ScL_Leaderboard");
    }
}
