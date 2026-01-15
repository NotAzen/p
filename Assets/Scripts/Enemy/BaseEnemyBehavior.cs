using UnityEngine;

public class BaseEnemyBehavior : MonoBehaviour
{
    // adjustable settings
    public float health = 100f;
    public float sightRange = 20f;
    public float speed = 5f;

    // line of sight to player
    private GameObject player;
    private bool hasLineOfSight = false;
    public float enemySlowing = 1f;

    // damage and explosion particles
    [SerializeField] private ParticleSystem damageParticles;
    private ParticleSystem damageParticlesInstance;

    [SerializeField] private ParticleSystem explosionParticles;
    private ParticleSystem explosionParticlesInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // enemy explosion whenever it dies
    public void ExplodeEnemy()
    {
        // play explosion particles
        explosionParticlesInstance = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        var explosionMain = explosionParticlesInstance.main;
        explosionMain.startColor = GetComponent<SpriteRenderer>().color;

        Destroy(gameObject);
    }

    // handle collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // player projectile layer is 8
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            // Assume the projectile has a ProjectileController script with a damage property
            ProjectileController projectile = collision.gameObject.GetComponent<ProjectileController>();

            // fail-safe check
            if (projectile != null)
            {
                health -= projectile.damage;
            }

            // play damage particles
            damageParticlesInstance = Instantiate(damageParticles, transform.position, Quaternion.identity);
            // define particle modules
            var main = damageParticlesInstance.main;
            var shape = damageParticlesInstance.shape;

            // set particle color to enemy color
            main.startColor = GetComponent<SpriteRenderer>().color;

            // orient particle shape towards collision point
            //Vector2 difference = (Vector2)transform.position - collision.contacts[0].point; <- based on contact point
            Vector2 difference = collision.relativeVelocity;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            shape.rotation = new Vector3(0, 90 - angle, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            ExplodeEnemy();
        }
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    private void FixedUpdate()
    {
        // define vector to player
        Vector3 toPlayerVector = player.transform.position - transform.position;

        // check line of sight to player
        RaycastHit2D ray = Physics2D.Raycast(transform.position,
                            toPlayerVector,
                            sightRange,
                            LayerMask.GetMask("Environment", "Player"));

        // if the raycast hits something, check if it's the player
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.gameObject == player;

            // move towards player if in line of sight
            if (hasLineOfSight)
            {
                GetComponent<Rigidbody2D>().linearVelocity = toPlayerVector.normalized * speed;
            }
            else
            {
                GetComponent<Rigidbody2D>().linearVelocity *= Mathf.Pow(enemySlowing, Time.deltaTime);
            }
            
            // draw debug ray
            Debug.DrawRay(transform.position,
                            toPlayerVector,
                            hasLineOfSight ? Color.green : Color.red);
        }
    }
}
