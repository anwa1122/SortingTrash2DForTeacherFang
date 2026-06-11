using UnityEngine;
using System.Collections.Generic;

public class Sc2_InventoryManager : MonoBehaviour
{
    public static Sc2_InventoryManager Instance;

    public List<Sc2_TrashData> items = new List<Sc2_TrashData>();
    public int trashCount;
    void Awake()
    {
        if (Instance == null) Instance = this;
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
