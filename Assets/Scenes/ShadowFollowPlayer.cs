using UnityEngine;

public class ShadowFollowPlayer : MonoBehaviour
{
    public Transform player;          // Assign the player's transform
    public float shadowOffset = 0.01f; // Offset to avoid z-fighting with the floor
    public LayerMask floorLayer;       // Layer mask to specify valid floor layers
    public float checkRadius = 0.5f;   // Radius of the area to check for floor detection

    private Renderer shadowRenderer;   // Renderer for the shadow object

    void Start()
    {
        // Get the renderer component to enable/disable the shadow rendering
        shadowRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Cast a sphere downwards from the player’s position to detect the floor within a radius
        RaycastHit hit;
        if (Physics.SphereCast(player.position, checkRadius, Vector3.down, out hit, Mathf.Infinity, floorLayer))
        {
            // Position the shadow at the hit point with the shadow offset
            transform.position = hit.point + Vector3.up * shadowOffset;

            // Enable the shadow rendering
            shadowRenderer.enabled = true;
        }
        else
        {
            // If no valid surface within radius, disable the shadow rendering
            shadowRenderer.enabled = false;
        }
    }
}
