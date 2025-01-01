using System.Collections;
using UnityEngine;

public class SpherePushTowardsPlayer : MonoBehaviour
{
    private Transform target; // The player's transform
    public float pushForce = 10f; // The force with which the sphere is pushed
    public float stopDelay = 0.5f; // Delay before the sphere starts moving again
    public float collisionPushForce = 50f; // Extra push force when colliding with the player


    private Rigidbody rb;
    private Vector3 lastTargetDirection;
    private bool isMoving = true;

    private void Start()
    {

         GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player object is tagged as 'Player'.");
        }

        rb = GetComponent<Rigidbody>();
        if (target != null)
        {
            // Initialize the last direction based on initial target position
            lastTargetDirection = (target.position - transform.position).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (target != null && isMoving)
        {
            // Calculate the current direction towards the target
            Vector3 currentDirection = (target.position - transform.position).normalized;

            // Check if the player has changed direction
            if (Vector3.Dot(lastTargetDirection, currentDirection) < 0.50f) // Threshold for direction change
            {
                StopAndResume(currentDirection);
            }
            else
            {
                // Apply force towards the target if there's no direction change
                rb.AddForce(currentDirection * pushForce, ForceMode.Acceleration);
            }
            
            // Update last known direction
            lastTargetDirection = currentDirection;
        }
    }

    private void StopAndResume(Vector3 newDirection)
    {
        // Stop the sphere's movement
        rb.velocity = Vector3.zero;
        isMoving = false;

        // Start the coroutine to resume movement after a delay
        StartCoroutine(ResumeMovement(newDirection));
    }

    private IEnumerator ResumeMovement(Vector3 newDirection)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(stopDelay);

        // Resume movement
        isMoving = true;
        rb.AddForce(newDirection * pushForce, ForceMode.Acceleration); // Push the sphere towards the new direction
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     // Check if the sphere has collided with the player
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            
    //         if (playerRb != null)
    //         {
    //             // Apply an extra burst of force to the player
    //             Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
    //             playerRb.AddForce(pushDirection * collisionPushForce, ForceMode.Impulse);
    //         }
    //     }
    // }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the sphere has collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Calculate the push direction based on the sphere's position
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;

                // Define the strength of the push
                float pushStrength = 40f; // Adjust this value for desired push intensity

                // Apply a force to move the player in the push direction
                playerRb.AddForce(pushDirection * pushStrength, ForceMode.Impulse);

                // Optionally, log the push event for debugging
                Debug.Log("Player pushed by rolling sphere!");
            }
        }
    }



    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
               // Calculate the direction of the push
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
                float pushStrength = 30f;

                // Get the player's current Rigidbody velocity
                Vector3 currentPlayerVelocity = playerRb.velocity;

                // Apply a force in the direction of the sphere, mimicking the sphere's movement direction
                // This will effectively 'glue' the player to the sphere's direction
                Vector3 gluedVelocity = pushDirection * pushStrength;

                // Combine the player's current velocity with the sphere's movement direction to simulate being glued
                playerRb.velocity = new Vector3(gluedVelocity.x, currentPlayerVelocity.y, gluedVelocity.z);

                Debug.Log("Player is glued to the sphere and moving in its direction!");
            }
        }
    }

}
