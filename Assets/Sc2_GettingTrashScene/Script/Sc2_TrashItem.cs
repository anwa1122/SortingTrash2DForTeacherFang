using UnityEngine;

public enum TrashType
{
    Recycle,
    General,
    Wet,
    Hazardous
}

[CreateAssetMenu(fileName = "New Trash", menuName = "Trash/Trash Data")]
public class Sc2_TrashItem : ScriptableObject
{
    public string trashName;
    public TrashType trashType;
    public int score;
    public Sprite icon;
}