using UnityEngine;
using UnityEngine.SceneManagement; // เพิ่มตัวนี้เข้ามาดูชื่อซีน

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string playerName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[GameManager] เริ่มทำงานครั้งแรกที่ซีน: {SceneManager.GetActiveScene().name}");
        }
        else
        {
            Debug.Log($"[GameManager] เจอตัวซ้ำในซีน {SceneManager.GetActiveScene().name} -> ทำลายตัวที่ซ้ำทิ้ง");
            Destroy(gameObject);
        }
    }

    // เพิ่มฟังก์ชันนี้เพื่อเช็คว่ามันตายตอนไหน
    private void OnDestroy()
    {
        // ถ้าตัวนี้เป็นตัวหลัก (Instance) แต่กำลังจะโดนทำลาย ให้แจ้งเตือน
        if (Instance == this)
        {
            Debug.LogWarning($"[GameManager] !!! ตัวหลักโดนทำลายแล้ว !!! ที่ซีน: {SceneManager.GetActiveScene().name}");
        }
    }
}