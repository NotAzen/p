using UnityEngine;

public class BaseEnemyBehavior : MonoBehaviour
{
    public float health = 100f;
    public float sightRange = 10f;

    private GameObject player;
    private bool hasLineOfSight = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // player projectile layer is 8
        if (collision.gameObject.layer == LayerMask.GetMask("PlayerBullets"))
        {
            // Assume the projectile has a ProjectileController script with a damage property
            ProjectileController projectile = collision.gameObject.GetComponent<ProjectileController>();

            // fail-safe check
            if (projectile != null)
            {
                health -= projectile.damage;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        // check line of sight to player
        RaycastHit2D ray = Physics2D.Raycast(transform.position,
                            player.transform.position - transform.position,
                            sightRange,
                            LayerMask.GetMask("Environment", "Player"));

        // if the raycast hits something, check if it's the player
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.gameObject == player;

            GetComponent<Rigidbody2D>.linearVelocity = hasLineOfSight
                ? (player.transform.position - transform.position).normalized * 2f
                : Vector2.zero;

            // draw debug ray
            Debug.DrawRay(transform.position,
                          player.transform.position - transform.position,
                          hasLineOfSight ? Color.green : Color.red);
        }
    }
}
