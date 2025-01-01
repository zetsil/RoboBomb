using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public Rigidbody playerRb;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 2f, -5f); // Initial camera offset
    public float smoothSpeed = 5f; // Camera smooth speed
    public float rotationSmoothTime = 0.12f; // Rotation smoothing time
    public float lookAheadDistance = 2f; // Distance to look ahead of the player
    public float lookAheadSpeed = 5f; // Speed at which camera adapts to the direction

    [Header("Dynamic Offset Settings")]
    public float maxHorizontalOffset = 2f;
    public float maxForwardOffset = 3f;
    public float offsetAdjustSpeed = 2.0f;
    public float maxRotationAngle = 40f;

    [Header("Map Boundaries")]
    public Vector3 minBounds; // Minimum boundary (x, y, z)
    public Vector3 maxBounds; // Maximum boundary (x, y, z)

    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;
    private Vector3 targetDynamicOffset;
    private Vector3 currentDynamicOffset;
    private Vector3 lookAheadOffset;

    private void LateUpdate()
    {
        if (!player || !playerRb)
        {
            Debug.LogWarning("Player or Rigidbody is not assigned!");
            return;
        }

        // Get the player's velocity
        Vector3 playerVelocity = playerRb.velocity;

        // Look Ahead effect based on the player's movement direction
        if (playerVelocity.magnitude > 0.1f)
        {
            // Player is moving
            Vector3 playerVelocityDirection = playerVelocity.normalized;
            lookAheadOffset = playerVelocityDirection * lookAheadDistance;
        }
        else
        {
            // Player is stationary, reset the look-ahead offset
            lookAheadOffset = Vector3.zero;
        }

        // Smooth transition for lookAheadOffset
        lookAheadOffset = Vector3.Lerp(lookAheadOffset, lookAheadOffset, lookAheadSpeed * Time.deltaTime);

        // Calculate dynamic offset based on velocity direction (left/right, forward/backward)
        Vector3 localVelocity = player.InverseTransformDirection(playerRb.velocity);
        if (playerVelocity.magnitude > 0.1f)
        {
            // Player is moving, adjust the dynamic offset
            targetDynamicOffset.x = Mathf.Clamp(localVelocity.x, -maxHorizontalOffset, maxHorizontalOffset);
            targetDynamicOffset.z = Mathf.Clamp(localVelocity.z, -maxForwardOffset, maxForwardOffset);
        }
        else
        {
            // Player is stationary, reset dynamic offset
            targetDynamicOffset = Vector3.zero;
        }

        // Smoothly transition the dynamic offset
        currentDynamicOffset = Vector3.Lerp(currentDynamicOffset, targetDynamicOffset, offsetAdjustSpeed * Time.deltaTime);

        // Calculate the target position of the camera based on the player's position, offset, and dynamic offset
        Vector3 targetPosition = player.position + player.rotation * offset + currentDynamicOffset + lookAheadOffset;

        // Clamp the target position to the defined boundaries
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.z, maxBounds.z);

        // Smoothly transition camera position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Calculate the desired rotation for the camera
        Vector3 movementDirection = playerRb.velocity.normalized; // Direction of movement
        if (movementDirection.magnitude > 0.1f)
        {
            // Player is moving, rotate the camera to follow the player's direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            // Limit rotation angle (clamping the rotation between -maxRotationAngle and maxRotationAngle)
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            if (angle > maxRotationAngle)
            {
                targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationAngle);
            }

            // Smoothly rotate the camera
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime * Time.deltaTime);
        }
        else
        {
            // Player is stationary, reset rotation to default
            Quaternion defaultRotation = Quaternion.LookRotation(player.forward, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, rotationSmoothTime * Time.deltaTime);
        }
    }
}
