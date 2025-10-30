using UnityEngine;

public class HUDOffset : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // ADJUSTABLE VARIABLES

    // offset multiplier for HUD elements
    [SerializeField] private float offsetMultiplier = 40f;

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    private GameObject player;       // reference to the player object
    private GameObject mainCamera;   // reference to the main camera
    private RectTransform hudElement; // reference to the HUD element's RectTransform
    private Vector3 initialPosition; // initial position of the HUD element

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find the player and main camera objects
        player = GameObject.FindWithTag("Player");
        mainCamera = GameObject.FindWithTag("MainCamera");

        // get the RectTransform component of the HUD element
        hudElement = GetComponent<RectTransform>();
        initialPosition = hudElement.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraToPlayer = offsetMultiplier * Vector3.ClampMagnitude(player.transform.position - mainCamera.transform.position, 1f);
        hudElement.localPosition = initialPosition - new Vector3(cameraToPlayer.x, cameraToPlayer.y, 0f);
    }
}
