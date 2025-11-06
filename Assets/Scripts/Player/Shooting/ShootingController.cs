using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    // references to pointer and projectile prefab
    [Header("References")]
    [SerializeField] public GameObject pointer;
    [SerializeField] private GameObject projectilePrefab;

    [Range(0f, 1f)]
    [SerializeField] private float pointerDistance;
    private Vector3 pointerOffset;

    // projectile shooting variables
    [Header("Shooting Properties")]
    private bool currentlyShooting = false;
    private float shootRequestTime;
    public float shootRequestBuffer = 0.2f;
    private float lastShootTime;
    public float shootCooldown = 0.5f;

    // bullet properties
    [Header("Projectile Properties")]
    public float projectileSpeed = 10f;
    public float projectileDamage = 25f;
    public int projectileBounces = 3;

    // cinemachine camera
    [Header("Camera Offsetting")]
    [SerializeField] private CinemachineFollow virtualCamera;
    public float maxCameraDistance = 5f;
    public float cameraSmoothing = 0.1f;

    // --------------------------------------------------------------------------------- //

    // dash input handler
    public void OnShoot(InputValue value)
    {
        // ummm idk apparently >0.5f for buttons detects presses so like yeah
        if (value.Get<float>() > 0.5f)
        {
            currentlyShooting = true;
            shootRequestTime = Time.time;
        }
    }

    // get mouse position relative to world
    private Vector3 GrabMousePosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0; // ensure the z-coordinate is 0

        return mouseWorldPosition;
    }

    private void ShootBullet(Vector3 shootingVector)
    {
        // instantiate projectile at player position
        GameObject projectile = ProjectilePool.Instance.GetFromPool();
        projectile.transform.rotation = pointer.transform.rotation;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = shootingVector * projectileSpeed;

        // reset shooting flag
        currentlyShooting = false;

        // update last shoot time
        lastShootTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse position relative to world
        Vector3 mouseWorldPosition = GrabMousePosition();

        // vector from player to mouse
        Vector3 shootingVector = mouseWorldPosition - transform.position;

        // move camera offset towards mouse (done before shootingVector is normalized to more follow the mouse)
        Vector3 cameraOffset = Vector3.Magnitude(shootingVector) > maxCameraDistance
            ? Vector3.ClampMagnitude(shootingVector, maxCameraDistance) // if shooting vector is bigger than the maximum camera distance, clamp it
            : shootingVector; // else just use the shooting vector
        virtualCamera.FollowOffset = Vector3.Lerp(virtualCamera.FollowOffset, cameraOffset, cameraSmoothing * Time.deltaTime);
        virtualCamera.FollowOffset.z = -10f; // ensure camera z-offset is correct

        // normalize shooting vector and scale to pointer distance
        shootingVector.Normalize();
        pointerOffset = shootingVector * pointerDistance;

        // set pointer position
        pointer.transform.position = transform.position + pointerOffset;

        // point pointer towards mouse
        float angle = Mathf.Atan2(pointerOffset.y, pointerOffset.x) * Mathf.Rad2Deg;
        pointer.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // attempt to shoot bullet if requested
        if (currentlyShooting && Time.time > lastShootTime + shootCooldown)
        {
            ShootBullet(shootingVector);
        }

        // reset shooting flag if buffer time exceeded
        if (Time.time > shootRequestTime + shootRequestBuffer)
        {
            currentlyShooting = false;
        }
    }
}
