using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTwords : MonoBehaviour
{
    private Transform target; // The player's Transform
    public float moveSpeed = 5f;    // Speed at which the object moves
    public float rotationSpeed = 10f; // Speed at which the object rotates towards the target

    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!canMove)
          return;
        // Get the direction towards the target (on X and Z axes only)
        Vector3 direction = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z).normalized;

        // Set the velocity to move towards the target at a constant speed
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 velocity = direction * moveSpeed;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // Preserve current Y velocity
        }

        RotateTowardsTarget();
    }

    void Update(){

    }


     void RotateTowardsTarget()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = target.position - transform.position;

        // Normalize the direction vector (important for consistent rotation)
        directionToPlayer.Normalize();

        // Create the rotation to make the X-axis face the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        // Smoothly rotate towards the player using Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
               
    }
}
