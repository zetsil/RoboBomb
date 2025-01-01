using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGameNamespace; // Import the namespace containing BombColision
using TMPro;


public class ExplodeClose : BombColision
{
    private Transform target; // The player's Transform
    [SerializeField]
    private float closeThreshold = 7f;

    private bool startCounting = false;
    private bool hitOneTime = false;


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
    void Update()
    {
        // stop movement here if desired
        if(hasExploded){
            MoveTwords movementScript = GetComponent<MoveTwords>();
            movementScript.enabled = false;

            // stop Rigidbody's movement
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero; // Stop all movement
                rb.angularVelocity = Vector3.zero; // Stop rotation, if any
            }
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                Destroy(sphereCollider, 0.3f); // Remove the collider after 1 second
            }
        }

        // Calculate the distance to the target
        float distance = Vector3.Distance(transform.position, target.position);
        UpdateTextPosition(); // Ensure correct initial placement

        // Check if the object is close to the target
        if (distance <= closeThreshold && !startCounting) // closeThreshold is a distance you define
        {
            Debug.Log("Object is close to the target");
            startCountDown();
    
        }
    }


    void startCountDown(){ // start the count down of the explosion
        startCounting = true;
        // Create a new GameObject for the TextMeshPro object
        textGO = new GameObject("TextObject");

        audioSource = GetComponent<AudioSource>();

        // Add TextMeshPro component to the new GameObject
        textObject = textGO.AddComponent<TextMeshPro>();

        // Set the text value
        textObject.text = "3";

        // Set the font size, alignment, and other properties
        textObject.fontSize = 20;
        textObject.alignment = TextAlignmentOptions.Center;

        // Optionally set additional properties such as color, scale, etc.
        textObject.color = Color.red;

        isCountingDown = false; // it will be set tu true in begin
        // Begin count down
        Begin();
    }

    void OnTriggerEnter(Collider other){
         if (other.gameObject.tag == "Player")
         {
        // Call the function "MyFunction" after 2 seconds
        Invoke("Explode", 0.3f);
         }
    }
    void OnTriggerStay(Collider other){

    if (other.gameObject.tag == "Player" && !isCountingDown && !hitOneTime)
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
        hitOneTime = true; // avoid more then one hit after explosion

        // Add force to player instead of applying it immediately
        playerScript.AddForce(direction * 2f);  // Store the force to apply later

        }
    

     if (other.gameObject.tag == "Floor" && !isCountingDown)
    {
        // Get the BoxCollider component from the floor object
        BoxCollider floorCollider = other.GetComponent<BoxCollider>();

        if (floorCollider != null)
        {
            // Make the collider smaller by reducing its size
            floorCollider.size = floorCollider.size * 0.5f; // Halves the collider size, for example

            // Optionally, adjust the center of the collider if needed
            floorCollider.center = new Vector3(floorCollider.center.x, floorCollider.center.y - 0.1f, floorCollider.center.z);
        }

        Collapse(other.gameObject);
        Destroy(other.gameObject,5f);
        Destroy(gameObject,2f);
    }

    if (other.gameObject.tag == "Bomb" && !isCountingDown)
     {
        BombColision bomb = other.GetComponent<BombColision>();
        if (bomb != null) // Check if the component exists
        {
            bomb.Explode();
        }
     }

      
 }
}
