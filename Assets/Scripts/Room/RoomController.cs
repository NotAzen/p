using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int EnemyCount()
    {
        return transform.GetChild(3).childCount;
    }
}
