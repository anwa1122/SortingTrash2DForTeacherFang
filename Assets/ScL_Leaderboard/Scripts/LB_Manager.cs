using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LB_Manager : MonoBehaviour
{
    [Header("--- UI Setup (เสก Prefab) ---")]
    public Transform contentParent;
    public GameObject leaderboardPrefab;

    private const string SAVE_KEY = "LocalGameLeaderboard_V2"; // เปลี่ยนคีย์เซฟเพื่อป้องกันข้อมูลสับสนกับตัวเก่า

    void Start()
    {
        CheckAndSaveRecentPlay();
        GenerateLeaderboardUI();
    }

    private void CheckAndSaveRecentPlay()
    {
        if (Sc3_SummaryManager.Instance != null && !string.IsNullOrEmpty(Sc3_SummaryManager.Instance.finalPlayerName))
        {
            string newName = Sc3_SummaryManager.Instance.finalPlayerName;
            int newScore = Sc3_SummaryManager.Instance.finalScore;

            // 🎒 ดึงข้อมูลจำนวนขยะคงเหลือล่าสุดที่ข้ามซีนมา
            int newTrashCount = 0;
            if (Sc2_InventoryManager.Instance != null)
            {
                newTrashCount = Sc2_InventoryManager.Instance.items.Count;
            }

            // บันทึกข้อมูลทั้งหมดลงเครื่อง
            SaveScoreToDevice(newName, newScore, newTrashCount);

            // 🔥 ล้างข้อมูลผู้เล่นเก่าออกเพื่อความท้าทายในรอบถัดไป
            if (Sc2_InventoryManager.Instance != null)
            {
                Sc2_InventoryManager.Instance.items.Clear();
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.playerName = "";
            }
        }
    }

    private void SaveScoreToDevice(string name, int score, int trashCount)
    {
        LeaderboardData currentData = LoadLeaderboardData();

        // เพิ่มข้อมูลคนล่าสุดที่มีค่าขยะเข้าไปด้วย
        currentData.list.Add(new LeaderboardEntry(name, score, trashCount));

        // เรียงลำดับจากคะแนนมากไปน้อย และดึงเอาแค่ Top 10
        currentData.list = currentData.list.OrderByDescending(x => x.score).Take(10).ToList();

        string json = JsonUtility.ToJson(currentData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private void GenerateLeaderboardUI()
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
                // 🔥 ส่งข้อมูลไปเซ็ตที่หน้าจอรวมถึงค่าขยะด้วย
                rowScript.SetRowData(currentRank, entry.playerName, entry.score, entry.trashCount);
            }

            currentRank++;
        }
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
}

// ----------------------------------------------------
// โครงสร้างข้อมูลที่อัปเดตให้รองรับการเซฟจำนวนขยะ
// ----------------------------------------------------
[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public int trashCount; // 🔥 เพิ่มตัวแปรเก็บขยะลงระบบ JSON

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