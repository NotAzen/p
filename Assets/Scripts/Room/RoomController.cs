using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int EnemyCount()
    {
        return transform.GetChild(3).childCount;
    }

    public void AssignOrigins(GameObject origin)
    {
        // iterate through each child to find the spawn points
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("SpawnPoint"))
            {
                Debug.Log($"assigned {origin.transform.position} to {transform.GetChild(i).transform.position}");

                transform.GetChild(i).GetComponent<RoomSpawner>().origin = origin;
            }
        }
    }
}
