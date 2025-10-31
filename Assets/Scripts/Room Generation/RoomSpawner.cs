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
    private int randomDoorIndex;

    private List<GameObject[]> roomClass;

    private bool spawned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find the RoomTemplates object in the scene
        templates = GameObject.FindWithTag("Rooms").GetComponent<RoomTemplates>();

        if (spawned) return;

        // skip spawning if limit reached
        if (templates.roomsSpawned < 30)
        {
            roomClass = new List<GameObject[]>
            {
                templates.bottomRooms,
                templates.topRooms,
                templates.leftRooms,
                templates.rightRooms
            };
            //Invoke("Spawn", 0.1f);
        }
        else
        {
            roomClass = new List<GameObject[]>
            {
                templates.bottomCaps,
                templates.topCaps,
                templates.leftCaps,
                templates.rightCaps
            };
            //Invoke("SpawnCaps", 0.1f);
        }

        // spawn after delay
        Invoke("Spawn", 0.1f);

        // increment rooms spawned count
        templates.roomsSpawned++;
    }

    // spawn rooms based on opening direction
    void Spawn()
    {
        switch (openingDirection)
        {
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

        spawned = true;
    }

    // instantiate room based on opening direction
    private void SpawnRoom(GameObject[] roomType)
    {
        randomDoorIndex = Random.Range(0, roomType.Length);
        Instantiate(roomType[randomDoorIndex], transform.position, Quaternion.identity);
        print("SPAWNED ROOM" + transform.position + roomType[randomDoorIndex].name);
    }

    // called when another collider enters this trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // force destroy spawn point if it:
        // 1. collides with another spawn point
        // 2. the other spawn point has already spawned a room
        // 3. this spawn point has a higher opening direction value than the other
        if (other.CompareTag("SpawnPoint"))
        {
            if (other.GetComponent<RoomSpawner>().spawned || GetComponent<RoomSpawner>().openingDirection > other.GetComponent<RoomSpawner>().openingDirection) {
                print("DESTOYED SPAWN: " + GetComponent<RoomSpawner>().openingDirection + transform.position);
                Destroy(gameObject);
            }
        }
    }
}
