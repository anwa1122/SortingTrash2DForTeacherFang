using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject[] trashPrefabs;
    public Transform[] spawnPoints;

    void Start()
    {
        List<GameObject> availableTrash = new List<GameObject>(trashPrefabs);

        foreach (Transform point in spawnPoints)
        {
            if (availableTrash.Count == 0) break;

            int index = Random.Range(0, availableTrash.Count);

            Instantiate(
                availableTrash[index],
                point.position,
                Quaternion.identity);

            availableTrash.RemoveAt(index);
        }
    }
}