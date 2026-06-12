using UnityEngine;
using TMPro;

public class LB_Row : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;
    public TMP_Text trashCountText; // 🔥 เพิ่ม Text สำหรับแสดงจำนวนขยะ

    // ฟังก์ชันสำหรับรับค่ามาบรรจุลงตัวหนังสือบนหน้าจอ (เพิ่มพารามิเตอร์ remainTrash)
    public void SetRowData(int rank, string playerName, int score, int remainTrash)
    {
        if (rankText != null) rankText.text = rank.ToString();
        if (nameText != null) nameText.text = playerName;
        if (scoreText != null) scoreText.text = score.ToString();
        if (trashCountText != null) trashCountText.text = remainTrash.ToString(); // 🔥 แสดงผลจำนวนขยะ
    }
}