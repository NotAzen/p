using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public float contactDamage = 25f;

    // if it collides with the player, deal damage
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // deal damage to player
            other.gameObject.GetComponent<PlayerController>().TakeDamage(contactDamage);
        }
    }
}
