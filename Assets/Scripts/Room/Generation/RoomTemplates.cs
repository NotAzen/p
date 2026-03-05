using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomSet
{
    public RoomCluster normalCluster;
    public RoomCluster endCluster;
    public RoomCluster winCluster;
}

[Serializable]
public class RoomCluster
{
    [SerializeField] private GameObject[] top;
    [SerializeField] private GameObject[] bottom;
    [SerializeField] private GameObject[] left;
    [SerializeField] private GameObject[] right;

    public List<GameObject[]> rooms
    {
        get {
            return new List<GameObject[]>
            {
                bottom,
                top,
                left,
                right
            };
        }

    }
}

[Serializable]
public class RoomSetEntry
{
    // Editable string index in the inspector
    public string key;
    public RoomSet set;
}

public class RoomTemplates : MonoBehaviour
{
    // reference to the starting room prefab
    [Header("Starting Room")]
    public GameObject startingRoom;

    [Header("Room Sets")]
    [Tooltip("room sets :3")]
    [SerializeField] private List<RoomSetEntry> roomSetEntries = new();

    // Runtime dictionary built from the serialized entries. Not serialized by Unity.
    public Dictionary<string, RoomSet> roomSets = new();

    private List<GameObject[]> _rooms;
    public List<GameObject[]> rooms
    {
        get { return _rooms; }
        private set { _rooms = value; }
    }

    private List<GameObject[]> _caps;
    public List<GameObject[]> caps
    {
        get { return _caps; }
        private set { _caps = value; }
    }
    
    private List<GameObject[]> _winRooms;
    public List<GameObject[]> winRooms
    {
        get { return _winRooms; }
        private set { _winRooms = value; }
    }

    [NonSerialized] public bool dictionaryIsBuilt = false;

    public static bool winSpawned = false;
    public static GameObject winSpawner;

    private void Awake()
    {
        // build the runtime dictionary from the serialized entries as early as possible
        BuildRoomSetDictionary();
        AssignRoomSet("LEVEL1");
    }

    private void Start()
    {
        // ensure dictionary is built (safe to call multiple times)
        BuildRoomSetDictionary();

        Debug.Log($"RoomTemplates: Awake and Start completed.");
        foreach (GameObject[] roomArray in rooms)
        {
            foreach (GameObject room in roomArray)
            {
                if (room == null)
                {
                    Debug.LogWarning("RoomTemplates: Found null prefab in rooms array.");
                }
                else
                {
                    Debug.Log($"RoomTemplates: Room prefab '{room.name}' is assigned.");
                }
            }
        }
    }

    /// <summary>
    /// Build or rebuild the runtime dictionary from the serialized roomSetEntries list.
    /// Trims keys, skips empty keys, and logs warnings for duplicates.
    /// </summary>
    public void BuildRoomSetDictionary()
    {
        if (roomSetEntries == null)
        {
            roomSets = new Dictionary<string, RoomSet>();
            return;
        }

        var dict = new Dictionary<string, RoomSet>(StringComparer.Ordinal);
        for (int i = 0; i < roomSetEntries.Count; i++)
        {
            var entry = roomSetEntries[i];
            if (entry == null)
            {
                Debug.LogWarning($"RoomTemplates: Null entry at index {i} in roomSetEntries.");
                continue;
            }

            var key = entry.key?.Trim();
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"RoomTemplates: Skipping entry at index {i} because key is null or empty.");
                continue;
            }

            if (dict.ContainsKey(key))
            {
                Debug.LogWarning($"RoomTemplates: Duplicate key '{key}' found in roomSetEntries (index {i}). Skipping duplicate.");
                continue;
            }

            dict[key] = entry.set;
        }

        Debug.Log($"RoomTemplates: Built roomSets dictionary with {dict.Count} entries from roomSetEntries.");

        roomSets = dict;
        dictionaryIsBuilt = true;
    }

    public void AssignRoomSet(string setName)
    {
        if (!roomSets.ContainsKey(setName))
        {
            Debug.LogError($"RoomTemplates: Room set '{setName}' not found in roomSets dictionary.");
            return;
        }

        Debug.Log($"RoomTemplates: Assigning room set '{setName}'.");

        RoomSet set = roomSets[setName];

        rooms = set.normalCluster.rooms;
        caps = set.endCluster.rooms;
        winRooms = set.winCluster.rooms;
    }
}
