using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Destroy this projectile after 4 seconds
        Destroy(gameObject, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
            {
            // Get the PlayerMovement script attached to the player
            PlayerMovement playerScript = other.GetComponent<PlayerMovement>();

            // Calculate the direction from the player to the projectile
            Vector3 direction = transform.position - other.transform.forward;

            // Reverse the direction to push the player away from the projectile
            // direction = -direction;
            
            // Normalize the direction
            direction.Normalize();

            // Call the hit method on the player script
            playerScript.hit(2);

            // Add force to player instead of applying it immediately
            playerScript.AddForce(direction * 2f);  // Store the force to apply later

            // Destroy the projectile
            Destroy(gameObject);
            }
    }
}
