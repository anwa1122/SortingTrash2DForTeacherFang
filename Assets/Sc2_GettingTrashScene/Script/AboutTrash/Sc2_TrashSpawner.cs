using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("Prefab แม่แบบขยะ (ที่มี Hitbox ใหญ่ และมีลูกอยู่ข้างใน)")]
    public GameObject baseTrashPrefab;

    [Header("ฐานข้อมูลขยะทั้งหมด")]
    public List<Sc2_TrashData> allTrashData;

    [Header("กลุ่มวัตถุขยะ / จุดเกิด บนหน้าฉาก")]
    public GameObject spawnPointsContainer;

    void Start()
    {
        // ตรวจสอบความพร้อมของข้อมูลสุ่ม
        if (allTrashData == null || allTrashData.Count == 0)
        {
            Debug.LogError("กรุณาใส่ Trash Data ในลิสต์ให้ครบถ้วน!");
            return;
        }

        if (spawnPointsContainer == null)
        {
            Debug.LogError("กรุณาลาก GameObject แม่ในฉาก (Container) มาใส่ด้วยครับ!");
            return;
        }

        // วนลูปสแกนหาวัตถุทุกตัวที่วางเรียงรายอยู่ภายใต้ spawnPointsContainer
        foreach (Transform targetObj in spawnPointsContainer.transform)
        {
            // 🎲 สุ่มเลือกข้อมูลขยะด้วยระบบถ่วงน้ำหนัก (Weighted Random) ตัวใหม่ที่แม่นยำกว่าเดิม
            Sc2_TrashData selectedData = GetRandomTrashByWeight();
            if (selectedData == null) continue;

            Transform trashMain = targetObj;

            // [เช็คระบบ] ถ้าวัตถุบนหน้าฉากเป็นแค่ "จุดเกิดว่างเปล่า" 
            if (targetObj.GetComponent<Sc2_TrashObject>() == null && baseTrashPrefab != null)
            {
                GameObject spawnedTrash = Instantiate(baseTrashPrefab, targetObj.position, Quaternion.identity, spawnPointsContainer.transform);
                trashMain = spawnedTrash.transform;

                // ตัวจุดเกิดเดิมบนฉากสั่งปิดตัวเพื่อซ่อนไว้
                targetObj.gameObject.SetActive(false);
            }

            // --- 📦 สเต็ปจัดการระบบข้อมูล (ตัวแม่คุม Hitbox) ---
            Sc2_TrashObject trashComponent = trashMain.GetComponent<Sc2_TrashObject>();
            if (trashComponent != null)
            {
                trashComponent.Init(selectedData);
            }

            // --- 🖼️ สเต็ปเปลี่ยนรูปภาพสุ่มไอคอนที่ (ตัวลูก) เท่านั้น ---
            if (trashMain.childCount > 0)
            {
                Transform visualChild = trashMain.GetChild(0);
                SpriteRenderer spriteRenderer = visualChild.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = selectedData.icon;
                }
                else
                {
                    Debug.LogWarning($"ตัวลูกของ {trashMain.name} ไม่มีคอมโพเนนต์ SpriteRenderer ครับ!");
                }
            }
            else
            {
                Debug.LogWarning($"วัตถุขยะชื่อ {trashMain.name} ไม่มีวัตถุลูกอยู่ข้างใน! รูปขยะจะไม่ถูกเปลี่ยนนะ");
            }
        }
    }

    // 🌟 ฟังก์ชันระบบสุ่มถ่วงน้ำหนักอัจฉริยะ (Weighted Random) แก้ไขบั๊กล็อกผลสุ่มตัวเดิม
    private Sc2_TrashData GetRandomTrashByWeight()
    {
        // 1. คำนวณหาผลรวมของโอกาสเกิด (Weight) ทั้งหมดในฐานข้อมูลก่อน
        float totalWeight = 0f;
        foreach (var data in allTrashData)
        {
            totalWeight += data.spawnChancePercentage;
        }

        // หากลืมตั้งค่าโอกาสเกิดไว้ในทุกๆ ไฟล์ ให้ระบบดึงตัวแรกมาเซฟบั๊ก
        if (totalWeight <= 0) return allTrashData[0];

        // 2. สุ่มตัวเลขตั้งแต่ 0 ถึง ผลรวมน้ำหนักจริงทั้งหมด
        float randomRoll = Random.Range(0f, totalWeight);
        float weightCounter = 0f;

        // 3. วนลูปเพื่อค้นหาว่าตัวเลขสุ่มตกอยู่ในช่วงน้ำหนักของขยะชิ้นไหน
        for (int i = 0; i < allTrashData.Count; i++)
        {
            weightCounter += allTrashData[i].spawnChancePercentage;

            if (randomRoll <= weightCounter)
            {
                return allTrashData[i]; // คืนค่าขยะตัวที่สุ่มได้ตามสัดส่วนค่าน้ำหนักจริง
            }
        }

        // กรณีฉุกเฉินกันระบบพลาด ให้ส่งตัวท้ายสุดของลิสต์ออกไป
        return allTrashData[allTrashData.Count - 1];
    }
}