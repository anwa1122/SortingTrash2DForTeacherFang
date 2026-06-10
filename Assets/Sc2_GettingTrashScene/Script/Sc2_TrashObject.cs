using UnityEngine;

public class TrashObject : MonoBehaviour
{
    public Sc2_TrashItem data;
    private void Start()
    {
        Debug.Log(data.trashName);
    }
}