using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class LB_Manager : MonoBehaviour
{
    [Header("--- UI Setup (เสก Prefab) ---")]
    public Transform contentParent;
    public GameObject leaderboardPrefab;

    private const string SAVE_KEY = "LocalGameLeaderboard_V2";

    void Start()
    {
        // 💡 โค้ดหลอกระบบ: แอดคะแนนจำลองเข้าไปเทส 3 คน (เซ็ตเสร็จแล้วลบออกได้ครั
        // สั่งโหลดมาเสกตามปกติ
        GenerateLeaderboardUI();
    }

    public void GenerateLeaderboardUI()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        LeaderboardData data = LoadLeaderboardData();

        int currentRank = 1;
        foreach (var entry in data.list)
        {
            GameObject rowObj = Instantiate(leaderboardPrefab, contentParent);

            LB_Row rowScript = rowObj.GetComponent<LB_Row>();
            if (rowScript != null)
            {
                rowScript.SetRowData(currentRank, entry.playerName, entry.score, entry.trashCount);
            }

            currentRank++;
        }
    }

    public static void SaveScoreToDevice(string name, int score, int trashCount)
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");
        LeaderboardData currentData = string.IsNullOrEmpty(json) ? new LeaderboardData() : JsonUtility.FromJson<LeaderboardData>(json);

        currentData.list.Add(new LeaderboardEntry(name, score, trashCount));
        currentData.list = currentData.list.OrderByDescending(x => x.score).Take(10).ToList();

        string updateJson = JsonUtility.ToJson(currentData);
        PlayerPrefs.SetString(SAVE_KEY, updateJson);
        PlayerPrefs.Save();
    }

    private LeaderboardData LoadLeaderboardData()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");
        if (string.IsNullOrEmpty(json)) return new LeaderboardData();
        return JsonUtility.FromJson<LeaderboardData>(json);
    }

    public void OnClickBackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

    [ContextMenu("Clear All Leaderboard Data")]
    public void ClearLeaderboardPrefs()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        Debug.Log("ล้างค่า Leaderboard เก่าในเครื่องเกลี้ยงแล้ว!");
    }

    // ฟังก์ชันสำหรับผูกกับปุ่มบนหน้าจอ UI เพื่อสั่งล้างตารางคะแนนตอนเล่นเกมจริง
    public void OnClickResetLeaderboard()
    {
        // 1. สั่งลบข้อมูลคีย์เซฟตารางคะแนนออกจากเครื่องถาวร
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        // 2. สั่งรันคำสั่งเสกหน้าจอใหม่ทันที เพื่อเคลียร์แถวตารางเก่าบนหน้าจอให้โล่ง
        GenerateLeaderboardUI();

        Debug.Log("ล้างตารางคะแนนสำเร็จแล้ว!");
    }

    public void OnClickBackToMenu()
    {
        if (Sc2_SceneTransition.Instance != null)
        {
            Sc2_SceneTransition.Instance.ChangeScene("Sc1_MenuGame");
        }
        else
        {
            // แฟลชเซฟกรณีลืมวางสคริปต์ทรานซิชันในซีน ให้วาร์ปแบบปกติแทนกันเกมค้าง
            LoadingScreen.LoadSceneWithLoadingScreen("Sc1_MenuGame"); //
        }
    }
}//Sc1_MenuGame

// 🔥 เติมก้อนโครงสร้างข้อมูลนี้กลับเข้ามาข้างล่างไฟล์ (ห้ามลืมเด็ดขาด) 🔥
[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public int trashCount;

    public LeaderboardEntry(string n, int s, int t)
    {
        playerName = n;
        score = s;
        trashCount = t;
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> list = new List<LeaderboardEntry>();
}