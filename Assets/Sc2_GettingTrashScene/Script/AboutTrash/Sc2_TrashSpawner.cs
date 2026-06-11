using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("Prefab แม่แบบ")]
    public GameObject baseTrashPrefab;

    [Header("ฐานข้อมูลขยะทั้งหมด")]
    public List<Sc2_TrashData> allTrashData;

    [Header("กลุ่มจุดเกิด")]
    public GameObject spawnPointsContainer; // เพิ่มช่องนี้เพื่อแยก Object จุดเกิดออกไปต่างหาก

    void Start()
    {
        // ตรวจสอบความพร้อมของข้อมูล
        if (baseTrashPrefab == null || allTrashData == null || allTrashData.Count == 0)
        {
            Debug.LogError("กรุณาใส่ Base Trash Prefab และ Trash Data ให้ครบถ้วน!");
            return;
        }

        if (spawnPointsContainer == null)
        {
            Debug.LogError("กรุณาลาก GameObject ที่เป็นตัวแม่ของจุดเกิด (SpawnPoints) มาใส่ในช่อง Spawn Points Container ด้วยครับ!");
            return;
        }

        // วนลูปหา 'ลูก' ทุกตัวที่อยู่ภายใต้ spawnPointsContainer ที่เราลากมาใส่
        foreach (Transform spawnPoint in spawnPointsContainer.transform)
        {
            // สุ่มเลือกขยะตามเปอร์เซ็นต์
            Sc2_TrashData selectedData = GetRandomTrashByPercentage();

            if (selectedData != null)
            {
                // สร้างไอเทมตรงตำแหน่งของวัตถุลูก
                GameObject spawnedTrash = Instantiate(
                    baseTrashPrefab,
                    spawnPoint.position,
                    Quaternion.identity);

                // ส่งข้อมูลเข้าไปเซ็ตค่าตัวละคร/ขยะ
                Sc2_TrashObject trashComponent = spawnedTrash.GetComponent<Sc2_TrashObject>();
                if (trashComponent != null)
                {
                    trashComponent.Init(selectedData);
                }
            }
        }
    }

    // ฟังก์ชันสุ่มแบบอิงเปอร์เซ็นต์ (0 - 100)
    private Sc2_TrashData GetRandomTrashByPercentage()
    {
        float randomRoll = Random.Range(0f, 100f);
        float cumulativePercentage = 0f;

        foreach (var data in allTrashData)
        {
            cumulativePercentage += data.spawnChancePercentage;

            if (randomRoll <= cumulativePercentage)
            {
                return data;
            }
        }

        return allTrashData[allTrashData.Count - 1];
    }
}