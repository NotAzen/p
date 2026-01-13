using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    // nearest multiple
    private int NearestMultiple(float num, int multiple)
    {
        return Mathf.RoundToInt(num / multiple) * multiple;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            NearestMultiple(Camera.main.transform.position.x, 2),
            NearestMultiple(Camera.main.transform.position.y, 2),
            transform.position.z);
    }
}
