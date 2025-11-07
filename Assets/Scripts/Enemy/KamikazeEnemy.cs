using UnityEngine;

public class KamikazeEnemy : MonoBehaviour
{
    private BaseEnemyBehavior baseBehavior;

    public float contactDamage = 25f;

    private void Start()
    {
        // grab base enemy behavior
        baseBehavior = GetComponent<BaseEnemyBehavior>();
    }

    // if it collides with the player, explode
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // deal damage to player
            bool damageConnected = other.gameObject.GetComponent<PlayerController>().TakeDamage(contactDamage);

            // explode after collision
            if (damageConnected)
            {
                baseBehavior.ExplodeEnemy();
            }
        }
    }
}
