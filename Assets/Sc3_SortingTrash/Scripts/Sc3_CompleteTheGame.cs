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
        if (Sc2_SceneTransition.Instance != null)
        {
            Sc2_SceneTransition.Instance.ChangeScene("ScL_Leaderboard");
        }
        else
        {
            // แฟลชเซฟกรณีลืมวางสคริปต์ทรานซิชันในซีน ให้วาร์ปแบบปกติแทนกันเกมค้าง
            LoadingScreen.LoadSceneWithLoadingScreen("ScL_Leaderboard"); //
        }
    }
}//ScL_Leaderboard
