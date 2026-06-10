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
            SceneManager.LoadScene("Sc2_GettingTrashScene");
        }
        else
        {
            Debug.Log("Please enter your name");
        }

    }
}