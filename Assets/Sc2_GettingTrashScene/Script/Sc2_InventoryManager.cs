using UnityEngine;
using System.Collections.Generic;

public class Sc2_InventoryManager : MonoBehaviour
{
    public static Sc2_InventoryManager Instance;

    public List<Sc2_TrashData> items = new List<Sc2_TrashData>();
    public int trashCount;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // สั่งให้ออบเจกต์นี้ (และตัวลูกของมันทั้งหมด) ห้ามโดนทำลายเวลาเปลี่ยนซีน
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ถ้าเกิดโหลดซีนใหม่แล้วพบว่ามี InventoryManager ตัวเก่าตามมาจากซีนที่แล้ว
            // ให้ทำลายตัวที่เพิ่งเกิดใหม่ทิ้งทันที เพื่อไม่ให้เกิดตัวซ้ำซ้อนในฉาก (Singleton Pattern)
            Destroy(gameObject);
        }
    }
    public void AddItem(Sc2_TrashData data)
    {
        items.Add(data);
        UpdateTrashCount();
    }

    public void UpdateTrashCount()
    {
        trashCount = items.Count;
        Debug.Log(trashCount);
    }
}
