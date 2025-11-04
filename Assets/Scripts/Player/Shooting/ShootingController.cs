using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    // references to pointer and projectile prefab
    [SerializeField] public GameObject pointer;
    [SerializeField] private GameObject projectilePrefab;

    [Range(0f, 1f)]
    [SerializeField] private float pointerDistance;
    private Vector3 pointerOffset;

    // projectile shooting variables
    private bool currentlyShooting = false;
    private float shootRequestTime;
    public float shootRequestBuffer = 0.2f;
    private float lastShootTime;
    public float shootCooldown = 0.5f;

    // bullet properties
    public float projectileSpeed = 10f;
    public float projectileDamage = 25f;
    public int projectileBounces = 3;

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
