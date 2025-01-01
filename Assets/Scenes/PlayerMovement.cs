using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets; 


public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public float moveSpeed = 5f;  // Speed of the capsule's movement
    public float jumpForce = 5f;  // Jump force (if you want the capsule to jump)

    private Rigidbody rb;  // Rigidbody component of the capsule
    private Transform playerTransform;

    private bool playerIsDead = false;
    public float fallMultiplier = 2.5f;  // Multiplier to make the object fall faster
    public float groundCheckDelay = 0f; // Delay before checking if grounded
    public int maxHealth = 100;
    public int currentHealth;
    private AudioSource audioSource;
    public  AudioClip hurt;
    public  AudioClip healClip;
    private float jumpBufferTime = 0.2f; 
    private float jumpBufferTimer = 0f;
    private List<Vector3> forcesToApply = new List<Vector3>(); // List to accumulate forces
    private float forceTimer = 0.0f; // Timer to control when to apply the force
    private float forceInterval = 0.2f; // Time interval to wait before applying forces (in seconds)
    public float coyoteTime = 0.2f; // Time allowed to jump after leaving the platform
    private float coyoteTimeCounter;
    private bool isJumping = false;
    private bool isGrounded = false;
    [SerializeField] private float groundCheckDistance = 2f;
    private float groundCheckRadius = 0.5f; // Radius of the sphere to detect ground

    [SerializeField] private Transform groundCheckTransform;

    private float sprintSpeed = 40;
    private List<GameObject> overlappingFloors = new List<GameObject>(); 

    private bool doubleJump = false;

    public GameObject poofDoubleJump ;
    public Transform poofPosition;

    private float jumpForceMultiplier = 1f;

    private ThirdPersonController thirdPersonController;

    void Start()
    {
        // Get the Rigidbody component attached to this object
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        playerTransform = GetComponent<Transform>();

        // Find the ThirdPersonController component in child GameObjects
        thirdPersonController = GetComponentInChildren<ThirdPersonController>();
         
        // set player hp 
        PlayerHealthBar.curHP = currentHealth;
        PlayerHealthBar.maxHP = maxHealth;

        audioSource = GetComponent<AudioSource>();
        GetComponent<Renderer>().enabled = false;

         // Check if poofDoubleJump and poofPosition are assigned
        if (poofDoubleJump == null)
        {
            Debug.LogError("poofDoubleJump GameObject not assigned in the Inspector!");
        }
        if (poofPosition == null)
        {
            Debug.LogError("poofPosition Transform not assigned in the Inspector!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(playerIsDead)
        // if the player is dead stop update to avoid calling 
        // falling condition over and over again
            return;
      
        // set player hp on the PlayerHealthBar
        PlayerHealthBar.curHP = currentHealth;
        isGrounded = IsGrounded();
        thirdPersonController.Grounded = isGrounded;


        // Check if the player is grounded
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time counter
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote time counter
        }

        if (playerTransform.position.y <= -10f)
        {
            // if the player falls game over 
            Main_Script.restartGame = true;  // Trigger Game Over
            playerIsDead = true;
        }

        if(currentHealth <= 0)
        {
            Collapse(gameObject);
            StartCoroutine(DelayedGameOver()); // Start the coroutine for delayed Game Over
            playerIsDead = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime; // Start buffer timer
        }


        // Increase the timer by the time passed since the last frame
        forceTimer += Time.deltaTime;
                
    }


    private IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(2f); // Wait for the specified delay
        Main_Script.restartGame = true; // Trigger Game Over after the delay
    }

    void FixedUpdate(){
              // Get input for movement (WASD or Arrow keys)
        float moveHorizontal = Input.GetAxis("Horizontal");  // A/D or Left/Right
        float moveVertical = Input.GetAxis("Vertical");  // W/S or Up/Down

        // Calculate movement speed based on sprint
        float movementSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            movementSpeed = sprintSpeed; // Increase speed for sprinting
        }

        // Apply movement based on input
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        rb.AddForce(movement * movementSpeed);
        

        // Modify Y velocity for both falling and jumping
        if (rb.velocity.y < 0 || rb.velocity.y > 0.1 && !isGrounded) 
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;

            if ((isGrounded && !isJumping) || coyoteTimeCounter > 0) // Check if grounded within buffer window
            {
                CheckGroundedAndJump();
                doubleJump = true;
                jumpBufferTimer = 0f; // Reset buffer after jump
            }else if(doubleJump)
            {
                Jump(0.6f);
                // Instantiate the poof effect at double jummp
                if (poofDoubleJump != null && poofPosition != null) // Check for null references!
                    {
                        GameObject pooof = Instantiate(poofDoubleJump, poofPosition.position, poofPosition.rotation);
                        Destroy(pooof, 1f);
                    }
                else
                    {
                        Debug.LogWarning("poofDoubleJump or poofPosition not assigned!"); // Helpful warning
                    }
                doubleJump = false;
            }
        }
       
        if (movement.magnitude > 0.01f)
        {
            rb.AddForce(movement * movementSpeed);
        }
        else
        {
            // Optional: Apply some damping or zeroing of the velocity
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Stops sliding on the x and z axis
        }


        // Check if the timer has reached the interval time
        if (forceTimer >= forceInterval)
        {
            ApplyAccumulatedForces();

            // Reset the timer after applying the force
            forceTimer = 0.0f;
        }
    }


    // Method to apply all accumulated forces
    private void ApplyAccumulatedForces()
    {
        Vector3 totalForceDirection = Vector3.zero;
        float totalForceMagnitude = 0f;

        if (forcesToApply.Count > 0)
        {
            // Use the first force's direction as the only direction
            totalForceDirection = forcesToApply[0].normalized;

            // Sum the magnitudes of all forces
            foreach (Vector3 force in forcesToApply)
            {
                totalForceMagnitude += force.magnitude;  // Accumulate the magnitudes
            }

            // Cap the total force magnitude if necessary
            float maxForce = 5f;
            if (totalForceMagnitude > maxForce)
            {
                totalForceMagnitude = maxForce;  // Cap the force magnitude
            }

            // Apply the total force in the direction of the first projectile
            Vector3 totalForce = totalForceDirection * totalForceMagnitude;
            rb.AddForce(totalForce, ForceMode.Impulse);

            // Clear the list of forces for the next accumulation period
            forcesToApply.Clear();
        }
    }

    public void hit(int damage){
        // this is called in the 
        currentHealth -= damage;
        audioSource.clip = hurt;
        audioSource.Play();
        if(currentHealth <0)
            currentHealth = 0;
    }

    public void heal(int heal){
        // this is called in the 
        currentHealth += heal;
        int health = currentHealth;
        // Optionally clamp the health to a maximum value
        currentHealth = Mathf.Min(health, maxHealth); // Example maximum health of 100
        audioSource.clip = healClip;
        audioSource.Play();
    }


    // Check if the player is grounded using a larger detection area
    public bool IsGrounded()
    {
        Vector3 raycastOrigin = groundCheckTransform != null ? groundCheckTransform.position : transform.position;

        // Perform a sphere cast downwards from the origin
        RaycastHit hit;
        if (Physics.SphereCast(raycastOrigin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance))
        {
            if (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("FixedFloor") || hit.collider.CompareTag("Floor2") || hit.collider.CompareTag("CanonEnemy")) // Check the tag of the hit collider
            {
                return true;
            }
        }
        return false;
    }


    private void OnDrawGizmosSelected()
    {
        // Get the raycast origin, handling the optional groundCheckTransform
        Vector3 raycastOrigin = groundCheckTransform != null ? groundCheckTransform.position : transform.position;

        // Set the gizmo color based on whether the player is grounded
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // Draw the ray
        Gizmos.DrawRay(raycastOrigin, Vector3.down * groundCheckDistance);

        // Optional: Draw a small sphere at the end of the ray to visualize the check point
        Gizmos.color = Color.blue; // A different color for the sphere
        Gizmos.DrawWireSphere(raycastOrigin + Vector3.down * groundCheckDistance, 0.02f); // Adjust sphere radius as needed
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Add the colliding floor tile to the list if it's not already included
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.tag == "FixedFloor" || collision.gameObject.tag == "CanonEnemy") 
        {
            if (!overlappingFloors.Contains(collision.gameObject)) 
            {
                overlappingFloors.Add(collision.gameObject); 
            }
            isGrounded = true; 
        }

        if (collision.gameObject.CompareTag("HealthPickup"))
        {
            HandleHealthPickup(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
        // if (collision.gameObject.CompareTag("Floor")) 
        // {
        //     // Remove the exiting floor tile from the list
        //     overlappingFloors.Remove(collision.gameObject); 
        // }
        // // Check if there are still any overlapping floor tiles
        // isGrounded = overlappingFloors.Count > 0; 
    }

    void CheckGroundedAndJump()
    {
        jumpForceMultiplier = 1f;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                jumpForceMultiplier = 1.4f; // Sprint jump boost
            }

            if (Input.GetKey(KeyCode.Space)) // Check for long jump during jump
            {
                jumpForceMultiplier *= 1.2f; // Long jump boost
            }

        Jump(jumpForceMultiplier);
    }

    void Jump(float jumpForceMultiplier)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce * jumpForceMultiplier, ForceMode.Impulse);
        isJumping = true;

    }


    private void HandleHealthPickup(GameObject pickup)
    {
        // Increase the player's health
        heal(3);

        // You might want to play a sound effect or visual effect here

        // Destroy the pickup object
        Destroy(pickup);

        Debug.Log("Health Picked Up! Current Health");
    }

    // Method to add forces to the player usualy the projectile push force
    public void AddForce(Vector3 force)
    {
        forcesToApply.Add(force);  // Accumulate forces
    }


    protected void Collapse(GameObject obj)
    {
        // Add Rigidbody and apply force/torque to the current object (root or child)
        AddRigidbodyAndForce(obj);

        // Recursively apply the collapse to all child objects
        foreach (Transform child in obj.transform)
        {
            Collapse(child.gameObject); // Recursive call for each child object
        }
    }

    // Helper function to add Rigidbody and apply force/torque
    void AddRigidbodyAndForce(GameObject obj)
    {
        // Add Rigidbody if the object doesn't already have one
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
        }

        // Create a more upward-biased random direction
        Vector3 upwardDirection = new Vector3(
            Random.Range(-0.5f, 0.5f),  // Small random variation on the X-axis
            1f,                         // Strong upward force on the Y-axis
            Random.Range(-0.5f, 0.5f)   // Small random variation on the Z-axis
        ).normalized;

        // Apply the force with an upward bias
        rb.AddForce(upwardDirection * Random.Range(200f, 400f)); // Adjust the force range as needed

        // Optionally: Add some random torque to make parts spin
        rb.AddTorque(Random.insideUnitSphere * Random.Range(10f, 50f));
    }

    
    

}
