using Unity.VisualScripting;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    // tracking the number of rooms spawned and the maximum allowed
    public int roomsSpawned = 0;
    public int enemiesSpawned = 0;
    public int minRooms = 30;
    private RoomSpawner startSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startSpawner = GameObject.FindWithTag("StartPoint").GetComponent<RoomSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RegenerateDungeon()
    {
        roomsSpawned = 0;
        enemiesSpawned = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        startSpawner.RegenerateRoom();
    }
}
