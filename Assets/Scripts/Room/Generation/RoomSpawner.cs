using System.Collections;
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
    private GameObject dungeon;
    private DungeonController dungeonController;
    private int randomDoorIndex;

    private List<GameObject[]> roomClass;

    private bool spawned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find the RoomTemplates object in the scene
        dungeon = GameObject.FindWithTag("Dungeon");
        templates = dungeon.GetComponent<RoomTemplates>();
        dungeonController = dungeon.GetComponent<DungeonController>();

        StartCoroutine(Generate());
    }
    private IEnumerator Generate()
    {
        // Wait until the objectToWaitFor is not null
        yield return new WaitUntil(() => templates.dictionaryIsBuilt);

        // the rooms are now available, you can safely access it here
        Debug.Log("rooms set up :3c");
        
        // if under the spawn requirement, spawn normal rooms
        if (dungeonController.roomsSpawned < dungeonController.minRooms)
        {
            roomClass = templates.rooms;
        }
        // once the spawn requirement is reached, only spawn cap rooms
        else
        {
            roomClass = templates.caps;
        }

        // spawn after delay
        Invoke("Spawn", 0.05f);
    }

    public void RegenerateRoom()
    {
        // don't regenerate if this is not the starting room
        // since the starting room is the only one that can guarantee a valid dungeon layout
        if (!gameObject.CompareTag("StartPoint"))
        {
            Debug.LogError("Cannot regenerate starting room!");
            return;
        }

        spawned = false;
        Invoke("Spawn", 0.05f);
    }

    // spawn rooms based on opening direction
    private void Spawn()
    {
        // if room already spawned, exit
        if (spawned) return;

        // by this point, this spawn point is spawning a room, so
        // if win room has not been spawned yet and this is a cap room spawn point
        if (!RoomTemplates.winSpawned && roomClass == templates.caps)
        {
            RoomTemplates.winSpawned = true;

            roomClass = templates.winRooms;
            Debug.Log("WIN ROOM SPAWNER SET AT: " + transform.position);
        }

        // increment rooms spawned count
        dungeonController.roomsSpawned++;

        // room spawning debugging
        Debug.Log(dungeonController.roomsSpawned);

        // indicate that a room has been spawned
        spawned = true;

        switch (openingDirection)
        {
            // spawn starting room
            case 0:
                Instantiate(templates.startingRoom, transform.position, Quaternion.identity, dungeon.transform);
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
        GameObject room = Instantiate(roomType[randomDoorIndex], transform.position, Quaternion.identity, dungeon.transform);

        dungeonController.enemiesSpawned += room.GetComponent<RoomController>().EnemyCount();

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
        if (other.CompareTag("SpawnPoint") || other.CompareTag("StartPoint"))
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
