using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // ADJUSTABLE SETTINGS

    [SerializeField] private float activeTime = 5f;
    private float timeActivated;

    [SerializeField] private ParticleSystem explosionParticles;
    private ParticleSystem explosionParticlesInstance;

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    private Transform player;
    private ShootingController shootingController;
    private Transform playerPointer;

    private int bounces;
    public float damage;

    private void OnEnable()
    {
        // initialize afterimage properties
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootingController = player.GetComponent<ShootingController>();
        playerPointer = shootingController.pointer.transform;

        // set afterimage properties based on player properties
        transform.position = playerPointer.position;
        transform.rotation = playerPointer.rotation;
        timeActivated = Time.time;

        // set bullet properties from shooting controller
        bounces = shootingController.projectileBounces;
        damage = shootingController.projectileDamage;
    }

    private void removeBullet()
    {
        // return projectile to pool
        ProjectilePool.Instance.AddToPool(gameObject);

        // play explosion particles
        explosionParticlesInstance = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(explosionParticlesInstance.gameObject, explosionParticlesInstance.main.duration);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // decrease bounces on collision
        bounces--;

        // pop bullet if out of bounces
        if (bounces < 0)
        {
            removeBullet();
        }

        // enemy layer is 10
        if (collision.gameObject.layer == LayerMask.GetMask("Enemy"))
        {
            // pop bullet on enemy hit
            removeBullet();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // pop bullet after active time
        if (Time.time >= timeActivated + activeTime)
        {
            removeBullet();
        }
    }
}
