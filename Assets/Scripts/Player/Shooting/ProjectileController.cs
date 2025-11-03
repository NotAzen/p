using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // ADJUSTABLE SETTINGS

    [SerializeField] private float activeTime = 1f;

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    private float timeActivated;
    private float alpha;

    private Transform player;
    private Transform playerPointer;

    private Color color;

    private void OnEnable()
    {
        // initialize afterimage properties
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerPointer = player.GetComponent<ShootingController>().pointer.transform;

        // set afterimage properties based on player properties
        transform.position = playerPointer.position;
        transform.rotation = playerPointer.rotation;
        timeActivated = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // return afterimage to pool after active time
        if (Time.time >= timeActivated + activeTime)
        {
            ProjectilePool.Instance.AddToPool(gameObject);
        }
    }
}
