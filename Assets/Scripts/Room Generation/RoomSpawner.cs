using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    // openingDirection indicates which door is needed
    public int openingDirection;
    /*
     * 1 --> need bottom door
     * 2 --> need top door
     * 3 --> need left door
     * 4 --> need right door
     */

    // reference to RoomTemplates
    private RoomTemplates templates;
    private DungeonController dungeon;
    private int randomDoorIndex;

    private List<GameObject[]> roomClass;

    private bool spawned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find the RoomTemplates object in the scene
        GameObject dungeonController = GameObject.FindWithTag("Dungeon");
        templates = dungeonController.GetComponent<RoomTemplates>();
        dungeon = dungeonController.GetComponent<DungeonController>();

        // if under the spawn requirement, spawn normal rooms
        if (dungeon.roomsSpawned < dungeon.minRooms)
        {
            roomClass = templates.rooms;
        }
        // once the spawn requirement is reached, only spawn cap rooms
        else
        {
            roomClass = templates.caps;
        }

        // spawn after delay
        Invoke("Spawn", 0.1f);
    }

    // spawn rooms based on opening direction
    void Spawn()
    {
        // if room already spawned, exit
        if (spawned) return;

        // increment rooms spawned count
        dungeon.roomsSpawned++;

        // room spawning debugging
        Debug.Log(dungeon.roomsSpawned);

        // indicate that a room has been spawned
        spawned = true;

        switch (openingDirection)
        {
            // spawn starting room
            case 0:
                Instantiate(templates.startingRoom, transform.position, Quaternion.identity);
                break;
            // spawn room with bottom door
            case 1:
                SpawnRoom(roomClass[0]);
                break;
            // spawn room with top door
            case 2:
                SpawnRoom(roomClass[1]);
                break;
            // spawn room with left door
            case 3:
                SpawnRoom(roomClass[2]);
                break;
            // spawn room with right door
            case 4:
                SpawnRoom(roomClass[3]);
                break;
            default:
                Debug.LogError("Invalid opening direction: " + openingDirection);
                break;
        }
    }

    // instantiate room based on opening direction
    private void SpawnRoom(GameObject[] roomType)
    {
        randomDoorIndex = Random.Range(0, roomType.Length);
        Instantiate(roomType[randomDoorIndex], transform.position, Quaternion.identity);
        
        // room spawning debugging
        Debug.Log("SPAWNED ROOM" + transform.position + roomType[randomDoorIndex].name);
    }

    // called when another collider enters this trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // skip if already spawned
        if (spawned) return;

        // force destroy spawn point if it:
        // 1. collides with another spawn point
        // 2. the other spawn point has already spawned a room
        // 3. this spawn point has a higher opening direction value than the other
        if (other.CompareTag("SpawnPoint"))
        {
            // the other spawn point has already spawned a room
            if (other.GetComponent<RoomSpawner>().spawned) {
                // room spawning debugging
                Debug.Log("DESTROYED SPAWN: " + GetComponent<RoomSpawner>().openingDirection + transform.position + " / REASON: ALREADY SPAWNED");
                
                Destroy(gameObject);
            }
            // this spawn point has a higher opening direction value than the other
            else if (GetComponent<RoomSpawner>().openingDirection > other.GetComponent<RoomSpawner>().openingDirection)
            {
                // room spawning debugging
                Debug.Log("DESTROYED SPAWN: " + GetComponent<RoomSpawner>().openingDirection + transform.position + " / REASON: HIGHER OPENING DIRECTION");
                
                Destroy(gameObject);
            }
        }
    }
}
