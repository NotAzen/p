using UnityEngine;

public class StaggeredMovement : MonoBehaviour
{
    // multiples for x and y axis
    [SerializeField] private int xMultiple = 2;
    [SerializeField] private int yMultiple = 2;

    // nearest multiple
    private int NearestMultiple(float num, int multiple)
    {
        return Mathf.RoundToInt(num / multiple) * multiple;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            NearestMultiple(Camera.main.transform.position.x, xMultiple),
            NearestMultiple(Camera.main.transform.position.y, yMultiple),
            transform.position.z);
    }
}
