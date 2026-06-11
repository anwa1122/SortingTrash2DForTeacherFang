using UnityEngine;
using TMPro;

public class Sc3_SummaryManager : MonoBehaviour
{
    public static Sc3_SummaryManager Instance;

    [Header("--- ส่วนดึงข้อมูล Text ในหน้าต่าง Summary ---")]
    public TMP_Text nameText;
    public TMP_Text scoreText;
    public TMP_Text trashCountText;

    // ตัวแปรเก็บค่าไว้ใช้ต่อใน Leaderboard
    [HideInInspector] public string finalPlayerName;
    [HideInInspector] public int finalScore;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 🔥 ฟังก์ชันหลักที่โดนเรียกจาก Controller ตอนจบเกม
    public void UpdateAndShowSummary(int score)
    {
        // 1. ดึงชื่อจาก GameManager
        finalPlayerName = GameManager.Instance != null ? GameManager.Instance.playerName : "Guest Player";
        finalScore = score;

        // 2. อัปเดตตัวหนังสือลง UI หน้าต่างสรุปผล
        if (nameText != null) nameText.text = $"Player Name: {finalPlayerName}";
        if (scoreText != null) scoreText.text = $"Score: {finalScore} points";
        if (trashCountText != null) trashCountText.text = $"Trash Count: {Sc2_InventoryManager.Instance.trashCount} ";

        // 3. สั่งสคริปต์ NotificationUI ให้ "ซูมหน้าต่างสรุปผลขึ้นมาโชว์"
        Sc3_NotificationUI.Instance.ShowSummaryWindow();
    }

    // 🔥 ฟังก์ชันที่คุณเอาไว้ผูกกับปุ่ม "ดู Leaderboard" หรือ "ไปต่อ" ในหน้าต่างสรุปผล
    public void OnClickGoToLeaderboard()
    {
        Debug.Log($"[Leaderboard] กำลังส่งชื่อ: {finalPlayerName} และ คะแนน: {finalScore} ไปจัดอันดับ...");

        // 🛠️ จุดนี้แหละที่คุณจะเอาไปเขียนต่อตอนทำ Leaderboard เช่น:
        // LeaderboardManager.Instance.SubmitScore(finalPlayerName, finalScore);
        // หรือ SceneManager.LoadScene("LeaderboardScene");
    }
}