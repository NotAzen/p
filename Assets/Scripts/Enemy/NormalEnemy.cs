using Unity.Cinemachine;
using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public float contactDamage = 25f;
    public Cooldown damageInterval = new(1f); // 1 second cooldown

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && damageInterval.IsReady())
        {
            // deal damage to player
            other.gameObject.GetComponent<PlayerController>().TakeDamage(contactDamage);
            damageInterval.Trigger();
        }
    }
}
