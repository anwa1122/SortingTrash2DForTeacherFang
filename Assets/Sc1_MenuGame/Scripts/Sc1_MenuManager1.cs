using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // จำเป็นต้องใช้สำหรับคอร์รูทีนแอนิเมชันของปุ่ม

public class MenuManager : MonoBehaviour
{
    [Header("--- UI Components ---")]
    public TMP_InputField nameInput;

    [Tooltip("ลากข้อความ TMP_Text ที่อยู่ข้างในปุ่ม Start Game มาใส่ช่องนี้เพื่อเปลี่ยนคำแจ้งเตือน")]
    public TMP_Text buttonText;

    [Header("--- Settings ---")]
    [Tooltip("คำดั้งเดิมของปุ่มก่อนที่จะเปลี่ยนเป็นคำเตือน (เช่น Start Game หรือ เข้าสู่เกม)")]
    public string defaultButtonText = "Start Game";

    private Color buttonDefaultColor = Color.white; // เซ็ตค่าเริ่มต้นเป็นสีขาวไว้ก่อนกันพลาด
    private Coroutine alertCoroutine;
    private Vector3 initialButtonScale;
    private RectTransform buttonRect;

    void Start()
    {
        // บันทึกคำเริ่มต้นและสีเริ่มต้นไว้ทันทีที่เปิดเกม ชัวร์ที่สุดครับ
        if (buttonText != null)
        {
            // บันทึกสีดั้งเดิมของปุ่มไว้ตรงนี้เลย ไม่ต้องรอเช็คเงื่อนไขอื่น
            buttonDefaultColor = buttonText.color;

            if (string.IsNullOrEmpty(defaultButtonText))
            {
                defaultButtonText = buttonText.text;
            }

            // หา RectTransform ของปุ่มเพื่อเอาไว้ทำเอฟเฟกต์เด้งเตือน
            buttonRect = buttonText.transform.parent.GetComponent<RectTransform>();
            if (buttonRect != null) initialButtonScale = buttonRect.localScale;
        }

        // 🎯 ดักฟังเหตุการณ์: เมื่อผู้เล่นกลับมาพิมพ์ในช่อง InputField ให้รีเซ็ตคำเตือนกลับเป็นปกติทันที
        if (nameInput != null)
        {
            nameInput.onValueChanged.AddListener(ResetButtonTextOnTyping);
        }
    }

    public void StartGame()
    {
        GameManager.Instance.playerName = nameInput.text;

        if (!string.IsNullOrWhiteSpace(nameInput.text))
        {
            ChangeToScene("Sc2_GettingTrash");
        }
        else
        {
            // สั่งให้ปุ่มเปลี่ยนข้อความเป็นคำเตือน พร้อมเอฟเฟกต์เด้งสั่นเตือนสายตา!
            if (buttonText != null)
            {
                if (alertCoroutine != null) StopCoroutine(alertCoroutine);
                alertCoroutine = StartCoroutine(ShowButtonAlertRoutine());
            }
        }
    }

    // 🎬 คอร์รูทีนเปลี่ยนคำเตือนบนปุ่ม พร้อมสั่นสเกลเบาๆ เตือนผู้เล่นว่าห้ามลืมกรอกชื่อน้า
    IEnumerator ShowButtonAlertRoutine()
    {
        buttonText.text = "Please enter your name!";
        buttonText.color = Color.red; // เปลี่ยนเป็นสีแดงเตือนสายตา

        // แถมเอฟเฟกต์เด้งกระตุกสั้นๆ เพิ่มความน่ารักและดึงดูดสายตา
        if (buttonRect != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float sinValue = Mathf.Sin(elapsed * Mathf.PI * 10f); // ลูปสั่นดึ๋งๆ
                buttonRect.localScale = initialButtonScale + new Vector3(sinValue * 0.1f, sinValue * 0.1f, 0f);
                yield return null;
            }
            buttonRect.localScale = initialButtonScale;
        }

        // บังคับโชว์คำเตือนค้างไว้ 2 วินาที ก่อนจะสลับกลับเป็นคำเดิมอัตโนมัติ
        yield return new WaitForSeconds(2f);

        // ดึงข้อความเดิมและสีดั้งเดิมกลับมาอย่างปลอดภัยร้อยเปอร์เซ็นต์
        buttonText.text = defaultButtonText;
        buttonText.color = buttonDefaultColor;
    }

    // ฟังก์ชันช่วยรีเซ็ตคำบนปุ่มทันทีเมื่อผู้เล่นเริ่มขยับมือพิมพ์ชื่อ
    void ResetButtonTextOnTyping(string text)
    {
        if (!string.IsNullOrWhiteSpace(text) && buttonText != null && buttonText.text == "Please enter your name!")
        {
            if (alertCoroutine != null) StopCoroutine(alertCoroutine);
            buttonText.text = defaultButtonText;
            buttonText.color = buttonDefaultColor;
            if (buttonRect != null) buttonRect.localScale = initialButtonScale;
        }
    }

    public void SeeLeaderBoard()
    {
        ChangeToScene("ScL_Leaderboard");
    }

    void ChangeToScene(string sceneName)
    {
        if (Sc2_SceneTransition.Instance != null)
        {
            Sc2_SceneTransition.Instance.ChangeScene(sceneName);
        }
        else
        {
            LoadingScreen.LoadSceneWithLoadingScreen(sceneName);
        }
    }

    void OnDestroy()
    {
        // คืนแรมล้าง EventListener ตอนทำลายวัตถุ
        if (nameInput != null)
        {
            nameInput.onValueChanged.RemoveListener(ResetButtonTextOnTyping);
        }
    }
}