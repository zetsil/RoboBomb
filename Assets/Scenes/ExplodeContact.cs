using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGameNamespace; // Import the namespace of AtackBot
public class ExplodeContact : MonoBehaviour
{


    private Rigidbody rb; // Reference to the Rigidbody component
    private SphereCollider sphereCollider; // Reference to the Sphere Collider component
    public bool isCountingDown = false;
    public GameObject explosion;

    protected AudioSource audioSource;
    protected bool hasExploded = false; // Flag to track if the bomb has exploded


    void Start()
    {    
        // Get the Rigidbody and SphereCollider components attached to this GameObject
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponents<SphereCollider>()[1];
        audioSource = GetComponent<AudioSource>();
    }


 void OnCollisionEnter(Collision other){
    if (other.gameObject.tag == "Floor")
    {
        print("enter");
    }

    }

 public void Explode(){
    isCountingDown = false;

    if(hasExploded)
        return;

    if (!audioSource.isPlaying && !hasExploded)
        audioSource.Play();

    // Step 1: Hide the bomb's visuals
    Renderer renderer = GetComponent<Renderer>();
    if (renderer != null)
    {
        renderer.enabled = false; // Hide the bomb visually
    }

    // Step 1.1: Hide all child renderers
    foreach (Renderer childRenderer in GetComponentsInChildren<Renderer>())
    {
        if (childRenderer != null)
        {
            childRenderer.enabled = false; // Hide child renderers
        }
    }

    // Step 2: Instantiate explosion effect at the bomb's position and rotation
    if(!hasExploded)
    {
        GameObject explosionInstance = Instantiate(explosion, transform.position, transform.rotation); // Create explosion
        explosionInstance.transform.SetParent(transform);  // Make the explosion a child of the bomb
    }


    // Step 3: Disable all non-audio components (like the Collider)
    Collider collider = GetComponent<Collider>();
    if (collider != null)
    {
        collider.enabled = false; // Disable the collider so it doesn't interact anymore
    }
    // Step 4: Trigger the screen shake
    if(!hasExploded)
        Camera.main.GetComponent<ScreenShake>().Shake();

    hasExploded = true;

    if (sphereCollider != null)
    {
        Destroy(sphereCollider, 1.0f); // Remove the collider after 1 second
    }

    Destroy(gameObject, 0.8f);
 }

 void OnTriggerEnter(Collider other){
    if (other.gameObject.tag == "Floor")
    {
        Explode();
    }
 }

 void OnTriggerExit(Collider other)
 {

 }


void OnTriggerStay(Collider other)
{
    if (other.gameObject.tag == "Bomb" && !isCountingDown)
     {
        BombColision bomb = other.GetComponent<BombColision>();
        if (bomb != null) // Check if the component exists
        {
            bomb.Explode();
        }
     }
    if (other.gameObject.tag == "RobotEnemy" && !isCountingDown)
      {
         // Check if the object has the AtackBot component (enemy)
        AtackBot enemy = other.gameObject.GetComponent<AtackBot>();

        if (enemy != null)
        {
            // If the enemy script is present, mark the enemy as dead
            enemy.Die();
        }

        Collapse(other.gameObject);
        Destroy(gameObject);
        Destroy(other.gameObject,2f);
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
      
 }


    void Collapse(GameObject obj)
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

    // OnDestroy is called when this GameObject (parent) is destroyed
    private void OnDestroy()
    {

    }

    void Update(){

        // Check if the object's Y position is less than -20
        if (transform.position.y < -20)
        {
            // Remove the object
            Destroy(gameObject);
        }
    }

}


