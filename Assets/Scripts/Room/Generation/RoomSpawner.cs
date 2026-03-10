using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    // openingDirection indicates which door is needed
    public List<int> openingDirections;
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

    [NonSerialized] public GameObject origin;
    private GameObject room;

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

    public void RegenerateDungeon()
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

    public void RegenerateRoom()
    {
        Destroy(room);
        Spawn();
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

        foreach (int openingDirection in openingDirections)
        {
            switch (openingDirection)
            {
                // spawn starting room
                case 0:
                    room = Instantiate(templates.startingRoom, transform.position, Quaternion.identity, dungeon.transform);
                    room.GetComponent<RoomController>().AssignOrigins(gameObject);
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
    }

    // instantiate room based on opening direction
    private void SpawnRoom(GameObject[] roomType)
    {
        randomDoorIndex = UnityEngine.Random.Range(0, roomType.Length);
        room = Instantiate(roomType[randomDoorIndex], transform.position, Quaternion.identity, dungeon.transform);

        dungeonController.enemiesSpawned += room.GetComponent<RoomController>().EnemyCount();

        // assign the origin of the room to this spawn point so that it can be accessed by the room's spawn points
        room.GetComponent<RoomController>().AssignOrigins(gameObject);

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
            // the other spawn point is the point is generated from, so this spawn point is redundant and should be destroyed
            if (other.gameObject == origin.GetComponent<RoomSpawner>().origin)
            {
                // room spawning debugging
                Debug.Log("DESTROYED SPAWN: " + GetComponent<RoomSpawner>().openingDirection + transform.position + " / REASON: ALREADY SPAWNED");

                Destroy(gameObject);
            }

            // the other spawn point has already spawned a room
            else if (other.GetComponent<RoomSpawner>().spawned) {
                //// room spawning debugging
                //Debug.Log("DESTROYED SPAWN: " + GetComponent<RoomSpawner>().openingDirection + transform.position + " / REASON: ALREADY SPAWNED");
                
                //Destroy(gameObject);

                Debug.Log("REGENERATED ROOM: " + GetComponent<RoomSpawner>().openingDirection + transform.position + " / REASON: COLLIDED WITH SPAWN THAT HAS ALREADY SPAWNED");

                origin.GetComponent<RoomSpawner>().RegenerateRoom();
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
