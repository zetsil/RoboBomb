using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0.5f; // How long the screen shake lasts
    public float shakeMagnitude = 0.1f; // How intense the shake is
    public Transform playerTransform; // Assign this in the Inspector

    private Vector3 originalPosition; // This will hold the position of the camera before shaking
    private bool isShaking = false; // Flag to check if shaking is active
    private CameraScript cameraScript;


    private void Start()
    {
        // Attempt to find the CameraScript component on this GameObject
        cameraScript = GetComponent<CameraScript>();
        
        // If the camera script is not found, try finding it in the scene
        if (cameraScript == null)
        {
            cameraScript = FindObjectOfType<CameraScript>();
            if (cameraScript == null)
            {
                Debug.LogError("CameraScript not found in the scene. Please assign it to this GameObject or ensure it's in the scene.");
            }
        }
    }

    // Public method to trigger the shake
    public void Shake()
    {
        if (playerTransform != null)
        {
            originalPosition = playerTransform.position + cameraScript.offset; // Get original position based on player
            StartCoroutine(ShakeCoroutine());
        }
        else
        {
            Debug.LogWarning("Player Transform is not assigned.");
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0.0f;

        isShaking = true; // Set shaking to true

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        isShaking = false; // Stop shaking after duration
    }

    private void LateUpdate()// use late update to not overide main camera script
    {
        if (isShaking)
        {
            originalPosition = playerTransform.position  + cameraScript.offset; // Get original position based on player
            // Generate random shake offsets
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // Apply the shake offset to the camera's position
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
        }
        else
        {
            // Reset to the new position based on player's position if not shaking
            transform.position = playerTransform.position  + cameraScript.offset;
        }
    }
}

