using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Sc2_TrashObject : MonoBehaviour
{
    public Sc2_TrashData data;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (data != null)
        {
            Init(data);
        }
    }

    public void Init(Sc2_TrashData newData)
    {
        data = newData;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (data.icon != null)
        {
            spriteRenderer.sprite = data.icon;
        }
        //Debug.Log($"สร้างขยะ: {data.trashName} ({data.spawnChancePercentage}%) เรียบร้อยแล้ว!");
    }
}