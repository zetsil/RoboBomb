using System.Collections.Generic;
using UnityEngine;

public class RandomHealthSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> healthPrefabs;
    [SerializeField] private string pickupTag = "HealthPickup"; // Tag for the instantiated objects

    void Start()
    {
        if (healthPrefabs.Count == 0)
        {
            Debug.LogError("No health prefabs assigned to spawn!");
            return;
        }

        // Choose a random prefab from the list
        int randomIndex = Random.Range(0, healthPrefabs.Count);
        GameObject prefabToInstantiate = healthPrefabs[randomIndex];

        // Instantiate the chosen prefab at the spawner's position and rotation
        GameObject spawnedObject = Instantiate(prefabToInstantiate, transform.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));


        // Add a Rigidbody component if one doesn't already exist
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
        }

        // Add a Capsule Collider if one doesn't already exist
        CapsuleCollider capsuleCollider = spawnedObject.GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            capsuleCollider = spawnedObject.AddComponent<CapsuleCollider>();
            //You might want to adjust the radius and height here if the default is not suitable
            // capsuleCollider.radius = 0.5f; //Example value
            // capsuleCollider.height = 1f; //Example value
        }

        // Add the specified tag
        spawnedObject.tag = pickupTag;
    }
}