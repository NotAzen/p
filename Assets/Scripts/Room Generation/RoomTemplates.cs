using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    // arrays of different room prefabs categorized by door openings
    [Header("Rooms")]
    public GameObject[] topRooms;
    public GameObject[] bottomRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    // arrays of cap prefabs for dead ends
    [Header("Dead Ends")]
    public GameObject[] topCaps;
    public GameObject[] bottomCaps;
    public GameObject[] leftCaps;
    public GameObject[] rightCaps;

    public int roomsSpawned = 0;
}
