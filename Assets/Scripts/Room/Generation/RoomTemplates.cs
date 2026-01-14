using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    // reference to the starting room prefab
    [Header("Starting Room")]
    public GameObject startingRoom;

    // arrays of different room prefabs categorized by door openings
    [Header("Rooms")]
    public GameObject[] topRooms;
    public GameObject[] bottomRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    private List<GameObject[]> _rooms;
    public List<GameObject[]> rooms
    {
        get { return _rooms; }
        private set { _rooms = value; }
    }

    // arrays of cap prefabs for dead ends
    [Header("Dead Ends")]
    public GameObject[] topCaps;
    public GameObject[] bottomCaps;
    public GameObject[] leftCaps;
    public GameObject[] rightCaps;
    private List<GameObject[]> _caps;
    public List<GameObject[]> caps
    {
        get { return _caps; }
        private set { _caps = value; }
    }

    // win room prefabs
    [Header("Win Rooms")]
    public GameObject[] winRoomsTop;
    public GameObject[] winRoomsBottom;
    public GameObject[] winRoomsLeft;
    public GameObject[] winRoomsRight;
    private List<GameObject[]> _winRooms;
    public List<GameObject[]> winRooms
    {
        get { return _winRooms; }
        private set { _winRooms = value; }
    }

    public static bool winSpawned = false;
    public static GameObject winSpawner;

    private void Start()
    {
        // initialize room categories
        rooms = new List<GameObject[]>
        {
            bottomRooms,
            topRooms,
            leftRooms,
            rightRooms
        };
        caps = new List<GameObject[]>
        {
            bottomCaps,
            topCaps,
            leftCaps,
            rightCaps
        };
        winRooms = new List<GameObject[]>
        {
            winRoomsBottom,
            winRoomsTop,
            winRoomsLeft,
            winRoomsRight
        };
    }
}
