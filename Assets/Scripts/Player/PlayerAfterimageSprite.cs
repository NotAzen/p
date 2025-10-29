using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class PlayerAfterimageSprite : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // ADJUSTABLE SETTINGS

    [SerializeField] private float activeTime = 1f;
    [SerializeField] private float initialAlpha = 1f;

    // --------------------------------------------------------------------------------- //
    // OTHER VARIABLES IDK

    private float timeActivated;
    private float alpha;

    private Transform player;

    private SpriteRenderer SR;
    private SpriteRenderer playerSR;

    private Color color;

    private void OnEnable()
    {
        // initialize afterimage properties
        SR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        // set afterimage properties based on player properties
        //alpha = alphaSet;
        SR.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        // fade out the afterimage over time
        float completionPercentage = (Time.time - timeActivated) / activeTime;
        alpha = Mathf.Lerp(initialAlpha, 0f, completionPercentage);
        color = new Color(1f, 1f, 1f, alpha);
        SR.color = color;

        // return afterimage to pool after active time
        if (Time.time >= timeActivated + activeTime)
        {
            PlayerAfterimagePool.Instance.AddToPool(gameObject);
        }
    }
}
