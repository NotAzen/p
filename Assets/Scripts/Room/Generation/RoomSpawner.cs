using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    // openingDirection indicates which door is needed
    public int openingDirection;
    public List<int> otherOpeningDirections = new();
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

    public GameObject origin;
    private GameObject room;

    public List<GameObject[]> roomClass;
    public GameObject[] roomsSpawnable;

    private bool spawned = false;

    public List<Vector2> wallChecks = new List<Vector2>()
    {
        new Vector2(0, -32), // bottom
        new Vector2(0, 32), // top
        new Vector2(-36, 0), // left
        new Vector2(36, 0), // right
    };

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
        if (dungeonController.roomsSpawned < dungeonController.minRooms || otherOpeningDirections.Count > 0)
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
        dungeonController.roomsSpawned--; // decrement rooms spawned count
        dungeonController.enemiesSpawned -= room.GetComponent<RoomController>().EnemyCount(); // reset enemy count

        spawned = false;
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

        if (openingDirection == 0)
        {
            // starting spawner
            room = Instantiate(templates.startingRoom, transform.position, Quaternion.identity, dungeon.transform);
            room.GetComponent<RoomController>().AssignOrigins(gameObject);

            // terminate early since it's the starting room
            return;
        }

        // assign room class to spawnable rooms based on assigned opening direction
        roomsSpawnable = roomClass[openingDirection - 1];

        for (int i = 0; i < wallChecks.Count; i++)
        {
            // exclude rooms that have a door in the direction of an adjacent wall
            if (Physics2D.Raycast((Vector2)transform.position + 0.5f * wallChecks[i], wallChecks[i], 0.01f, LayerMask.GetMask("Environment")))
            {
                int direction = i + 1;
                roomsSpawnable = roomsSpawnable.Except(roomClass[i]).ToArray();
            }
        }

        //Physics2D.BoxCast(Vector3.Lerp(origin.transform.position, transform.position, 0.5f),
        //        new Vector2(0.25f, 0.25f), 0f, Vector2.right, 0.01f, LayerMask.GetMask("Environment"))

        foreach (int otherOpeningDirection in otherOpeningDirections)
        {
            // include rooms that have a door in the direction of other spawn points that this spawn point collides with
            roomsSpawnable = roomsSpawnable.Intersect(roomClass[otherOpeningDirection - 1]).ToArray();
        }

        SpawnRoom(roomsSpawnable);
    }

    // instantiate room based on opening direction
    private void SpawnRoom(GameObject[] roomType)
    {
        // debugging to make sure intersecting room classes is working properly
        string availableRooms = "";
        foreach (GameObject room in roomType)
        {
            availableRooms += room.name + ", ";
        }
        Debug.Log("SPAWNABLE ROOMS: " + availableRooms);
        Debug.Log(transform.position + " / OPENING DIRECTION: " + openingDirection + " / OTHER OPENING DIRECTIONS: " + string.Join(", ", otherOpeningDirections) + " / SPAWNED ROOMS: " + dungeonController.roomsSpawned);

        // ------------------------------

        randomDoorIndex = UnityEngine.Random.Range(0, roomType.Length);
        Debug.Log("RANDOM DOOR INDEX: " + randomDoorIndex);
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
        if (!other.CompareTag("SpawnPoint") && !other.CompareTag("StartPoint")) return;

        // the other spawn point is the point is generated from, so this spawn point is redundant and should be destroyed
        if (other.gameObject == origin.GetComponent<RoomSpawner>().origin)
        {
            // room spawning debugging
            Debug.Log("DESTROYED SPAWN: " + GetComponent<RoomSpawner>().openingDirection + transform.position + " / REASON: ALREADY SPAWNED");

            Destroy(gameObject);
        }

        // the other spawn point has already spawned a room, so this spawn point is redundant and should be destroyed
        else if (other.GetComponent<RoomSpawner>().spawned)
        {
            Debug.Log("DESTROYED SPAWN: " + GetComponent<RoomSpawner>().openingDirection + transform.position + " / REASON: COLLIDED WITH SPAWN THAT HAS ALREADY SPAWNED");
            Destroy(gameObject);
        }

        // this spawn point has a higher opening direction value than the other
        else if (GetComponent<RoomSpawner>().openingDirection > other.GetComponent<RoomSpawner>().openingDirection)
        {
            // room spawning debugging
            Debug.Log("DESTROYED SPAWN: " + GetComponent<RoomSpawner>().openingDirection + transform.position + " / REASON: HIGHER OPENING DIRECTION");

            other.GetComponent<RoomSpawner>().otherOpeningDirections.Add(openingDirection);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (origin == null)
        {
            return;
        }
        
        Debug.DrawLine(Vector3.Lerp(origin.transform.position, transform.position, 0.5f), Vector3.Lerp(origin.transform.position, transform.position, 0.5f) + Vector3.right * 0.1f, Color.green);
    }
}
