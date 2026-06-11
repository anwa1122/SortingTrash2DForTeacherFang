using System.Collections.Generic;
using UnityEngine;

public class Sc3_DebugHelper : MonoBehaviour
{
    [Header("ใส่ขยะจำลองที่อยากให้มีตอนเริ่ม Scene 3")]
    public List<Sc2_TrashData> debugTrashItems;

    void Start()
    {
        // เรียกทำงานทันทีตอนเริ่มซีน
        AddMockData();
    }

    void AddMockData()
    {
        if (Sc2_InventoryManager.Instance != null)
        {
            // ตรวจสอบว่าในกระเป๋าว่างเปล่าจริงไหม (ป้องกันการทับข้อมูลจริงเวลาเล่นมาจากซีนอื่น)
            if (Sc2_InventoryManager.Instance.items.Count == 0)
            {
                Debug.Log("[Debug] กำลังเติมขยะจำลองเข้า Inventory เพื่อใช้ทดสอบในซีน 3...");

                foreach (var trash in debugTrashItems)
                {
                    Sc2_InventoryManager.Instance.AddItem(trash);
                }
            }
        }
        else
        {
            Debug.LogError("[Debug Error] ไม่พบ InventoryManager ในซีน! กรุณาสร้างออบเจกต์ InventoryManager ทิ้งไว้ในซีน 3 ด้วยครับ");
        }
    }
}