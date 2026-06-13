using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sc3_SortingGameController : MonoBehaviour
{
    [Header("Ui setting")]
    public GameObject trashPrefab;      // UI Prefab ขยะที่มีสคริปต์ Sc3_Dragable และ Image Component
    public Transform itemFather;         // พื้นที่สำหรับวางขยะตอนเริ่มเกม
    public RectTransform spawnArea;      // ขอบเขตที่จะใช้สุ่มพิกัดขยะ
    public Button confirmBtn;

    [Header("ใส่ถังขยะ UI ทั้งหมดในฉาก")]
    public List<Sc3_TrashSlot> trashSlots;

    private List<GameObject> spawnedTrashes = new List<GameObject>();
    private int finalScore = 0;

    void Start()
    {
        // เรียกให้เกมเริ่มทำงานทันทีเมื่อเปิดซีนนี้
        SetUpMiniGame();
    }

    public void SetUpMiniGame()
    {
        if (Sc2_InventoryManager.Instance == null || Sc2_InventoryManager.Instance.items.Count == 0)
        {
            Debug.LogWarning("ไม่มีข้อมูลขยะใน Inventory! เกมไม่ได้สร้างไอเทม");
            return;
        }

        // วนลูปสร้างไอเทมขยะตามข้อมูลที่มีอยู่ใน Inventory ของผู้เล่น
        foreach (Sc2_TrashData data in Sc2_InventoryManager.Instance.items)
        {
            GameObject obj = Instantiate(trashPrefab, itemFather);

            // ส่งข้อมูล Data ยัดใส่สคริปต์ลากวาง
            Sc3_Dragable dragScript = obj.GetComponent<Sc3_Dragable>();
            if (dragScript != null)
            {
                dragScript.data = data;
            }

            // เปลี่ยนภาพสไปรต์ตามข้อมูลขยะ
            Image img = obj.GetComponent<Image>();
            if (img != null && data.icon != null)
            {
                img.sprite = data.icon;
            }

            // สุ่มตำแหน่งกระจายตัวในพื้นที่ spawnArea
            obj.transform.localPosition = GetRandomPosInArea();
            spawnedTrashes.Add(obj);
        }
    }

    private Vector3 GetRandomPosInArea()
    {
        // ปรับรัศมีสุ่มเล็กน้อยเพื่อไม่ให้ขยะสุ่มชิดขอบจอเกินไป
        float x = Random.Range(-spawnArea.rect.width / 2.5f, spawnArea.rect.width / 2.5f);
        float y = Random.Range(-spawnArea.rect.height / 2.5f, spawnArea.rect.height / 2.5f);
        return new Vector3(x, y, 0);
    }

    // 🔥 ฟังก์ชันสำหรับผูกเข้ากับ "ปุ่มส่งคำตอบ/นับคะแนน" (Button OnClick)
    public void CheckAnswersAndEndGame()
    {
        if (itemFather.transform.childCount != 0)
        {
            Sc3_NotificationUI.Instance.ShowNotice("There are some left");
            return;
        }

        confirmBtn.interactable = false;


        finalScore = 0;
        int correctCount = 0;
        int wrongCount = 0;

        // วนลูปเช็คถังขยะแต่ละใบที่มีอยู่ในฉาก
        foreach (Sc3_TrashSlot slot in trashSlots)
        {
            // ดึงขยะทุกชิ้นที่ถูกลากเข้ามาใส่ในถังใบนี้ (หาจากลูกๆ ของมัน)
            Sc3_Dragable[] trashesInSlot = slot.GetComponentsInChildren<Sc3_Dragable>();

            foreach (Sc3_Dragable trash in trashesInSlot)
            {
                // ตรวจสอบว่าประเภทของขยะ ตรงกับประเภทของถังหรือไม่
                if (trash.data.trashType == slot.slotType)
                {
                    finalScore += trash.data.score; // บวกคะแนนตามที่ตั้งไว้ใน TrashData
                    correctCount++;
                }
                else
                {
                    // กรณีทิ้งขยะผิดถัง (สามารถปรับให้หักคะแนนตรงนี้เพิ่มได้ครับ)
                    wrongCount++;
                }
            }
        }

        Debug.Log($"--- สรุปผลเกมแยกขยะ --- \nคะแนนรวมทั้งหมด: {finalScore} แต้ม | แยกถูก: {correctCount} ชิ้น | แยกผิด: {wrongCount} ชิ้น");

        // สั่งพิมพ์ข้อความ Well Done ทันที
        Sc3_NotificationUI.Instance.ShowNotice("Well Done!!!");

        // 🔥 แทนที่จะสั่งเปิด Summary เลยตรงๆ ให้ส่งไปทำงานใน Coroutine หน่วงเวลาแทน
        StartCoroutine(WaitAndShowSummary());
        // ตรงนี้สามารถใส่โค้ดเปิดหน้าต่าง UI สรุปผล (Victory Screen) หรือเปลี่ยนซีนถัดไปได้เลยครับ
    }
    private IEnumerator WaitAndShowSummary()
    {
        // ⏳ สั่งให้หยุดรอตรงนี้เป็นเวลา 1 วินาที
        yield return new WaitForSeconds(1f);

        Sc3_CompleteTheGame.Instance.RunCompleteGame(finalScore);
        //Sc3_SummaryManager.Instance.UpdateAndShowSummary(finalScore);
    }
}