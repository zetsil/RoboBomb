using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MyGameNamespace{

public class AtackBot : MonoBehaviour
{
    private Transform player; // The player's Transform

    public float moveSpeed = 5f; // The speed at which the object will move
    public float stoppingDistance = 1.5f; // Distance to maintain from the player

    private float rotationSpeed = 6f;
    public bool isDead = false;
    private bool isOnFloor = false;

    public event Action OnEnemyDeath;


    // Start is called before the first frame update
    void Awake()
    {

        // Find the player by tag
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player object is tagged as 'Player'.");
        }
    }

    void Start(){
        // Subscribe EnemyManager to enemy's death event
        this.OnEnemyDeath += EnemyManager.Instance.HandleEnemyDeath;
    }

    // Update is called once per frame
    private void Update()
    {
                // Destroy the enemy if it falls below a certain Y position (e.g., fell off the map)
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }

    }


     // Method to rotate the object towards the player


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor") && !isOnFloor) // Using CompareTag for better performance
            {
                Debug.Log("Entered Floor Collider!");
                isOnFloor = true; // Set isOnFloor directly
                // navAgent.enabled = true;
            }
    }

    public void Die()
    {
        if(isDead)// avoid callind two times if bomb explodes
            return;
        isDead = true;
        DeactivateAllScriptsOnSelfAndChildren();

    }


    public void DeactivateAllScriptsOnSelfAndChildren()
    {
        // Deactivate scripts on the parent GameObject
        MonoBehaviour[] parentScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in parentScripts)
        {
            script.enabled = false;
        }

        // Deactivate scripts on child GameObjects
        foreach (Transform child in transform)
        {
            MonoBehaviour[] childScripts = child.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in childScripts)
            {
                script.enabled = false;
            }
        }
    }

    void OnDestroy()
    {
        Die();
        OnEnemyDeath?.Invoke();  // Trigger the event to notify the EnemyManager
        // Any other cleanup code can go here
    }

    

    
}
}
