using UnityEngine;

public enum TrashType
{
    Recycle,
    General,
    Organic,
    Hazardous,
    Infectious
}

[CreateAssetMenu(fileName = "New Trash", menuName = "Trash/Trash Data")]
public class Sc2_TrashData : ScriptableObject
{
    public string trashName;
    public TrashType trashType;
    public int score;
    public Sprite icon;

    [Range(0f, 100f)]
    [Tooltip("โอกาสเกิดเป็นเปอร์เซ็นต์ (0 - 100) แนะนำให้รวมกันทุกไฟล์ได้ 100%")]
    public float spawnChancePercentage = 25f; // ใช้เป็น float เผื่ออยากได้ทศนิยม เช่น 0.5%
}