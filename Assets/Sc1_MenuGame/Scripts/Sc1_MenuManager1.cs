using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void StartGame()
    {
        GameManager.Instance.playerName = nameInput.text;

        if (!string.IsNullOrWhiteSpace(nameInput.text))
        {
            LoadingScreen.LoadSceneWithLoadingScreen("Sc2_GettingTrash");
        }
        else
        {
            Debug.Log("Please enter your name");
        }

    }

    public void SeeLeaderBoard()
    {
        LoadingScreen.LoadSceneWithLoadingScreen("ScL_Leaderboard");
    }
}