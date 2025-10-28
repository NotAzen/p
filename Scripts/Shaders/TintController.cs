using UnityEngine;
using UnityEngine.Tilemaps;

public class TintController : MonoBehaviour
{
    // player
    public Transform playerTransform;

    // The Tilemap Renderer to get the material from
    private TilemapRenderer tilemapRenderer;

    void Start()
    {
        // Get the TilemapRenderer component
        tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    void Update()
    {
        // Pass the player's world position to the shader
        tilemapRenderer.material.SetVector("_PlayerPosition", playerTransform.position);
    }
}
