using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterimagePool : MonoBehaviour
{
    // prefab for the afterimage object
    [SerializeField] private GameObject afterimagePrefab;

    // queue to hold available afterimage objects
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    // singleton instance
    public static PlayerAfterimagePool Instance { get; private set; }

    // initialize the singleton instance and grow the pool at start
    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    // grow the pool by instantiating 10 new afterimage objects :3
    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(afterimagePrefab);
            obj.transform.SetParent(transform);
            AddToPool(obj);
        }
    }

    // add an object back to the pool
    public void AddToPool(GameObject obj)
    {
        obj.SetActive(false);
        availableObjects.Enqueue(obj);
    }

    public GameObject GetFromPool()
    {
        // if no objects are available, grow the pool
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }

        // dequeue an object and activate it
        GameObject obj = availableObjects.Dequeue();
        obj.SetActive(true);
        return obj;
    }
}
