using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;

    [Header("Camera Rigidbody Reference")]
    [SerializeField] private Rigidbody2D cameraRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toPlayerVector = playerTransform.position - gameObject.transform.position;

        cameraRigidbody.linearVelocity = toPlayerVector * 2f;

        //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, playerTransform.position, 2f * Time.deltaTime);
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10f);
    }
}
