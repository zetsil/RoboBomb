using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGameNamespace;

public class shootProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;  // The projectile prefab to be instantiated
    public float projectileSpeed = 20f;  // Speed at which the projectile moves
    public Transform shootPoint;  // The point from where the projectile will be fired
    public float shootInterval = 1f;  // Time interval between each shot (1 second)

    private float shootTimer = 0f;  // Timer to track when to shoot

    void Start()
    {
        // Check if a projectile prefab is assigned
        if (projectilePrefab == null)
        {
            Debug.LogError("No projectile prefab assigned. Please assign a prefab in the inspector.");
        }
    }

    void Update()
    {
        // Update the timer by the time passed since the last frame
        shootTimer += Time.deltaTime;
        AtackBot enemy = gameObject.GetComponent<AtackBot>();
        // If 1 second or more has passed, shoot and reset the timer
        if (shootTimer >= shootInterval && !enemy.isDead)
        {
            Shoot();
            shootTimer = 0f;  // Reset the timer
        }
    }

    void Shoot()
    {
        
        // Get the original rotation from the shoot point and apply an additional -90 degrees on the Y-axis
        Quaternion adjustedRotation = shootPoint.rotation * Quaternion.Euler(0, 0, 90);

        // Add an upward vector to target more in the midle
        Vector3 upwardAdjustment = Vector3.up * 1.3f; // Adjust the multiplier to change the upward force

        // Instantiate the projectile with the adjusted rotation
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position + upwardAdjustment, adjustedRotation);

        // Get the Rigidbody component of the projectile (assuming it has one)
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
        // Calculate the direction in the local X-axis (relative to the object's rotation)
        Vector3 localXDirection = transform.right;  // Local X-axis direction

        // Apply velocity to the projectile in the local X direction
        rb.velocity = localXDirection * projectileSpeed;

        }
    }


}
