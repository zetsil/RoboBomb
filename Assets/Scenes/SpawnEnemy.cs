using System.Collections;
using UnityEngine;
using UnityEngine.VFX;


public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;  // Assign the enemy prefab here
    public float spawnInterval = 5f; // Time in seconds between each spawn
    private bool isSpawning = true;
    public GameObject poofEffect;// this will spawn when an enemy apeare 

    private void Start()
    {
        // Start the spawning coroutine
        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        // Check if enemyPrefab is assigned before proceeding
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab is not assigned. Aborting spawn routine.");
            yield break; // Exit the coroutine if enemyPrefab is not set
        }

         // Wait until LevelManager.Instance.spawn_enemys is true
        while (!LevelManager.Instance.spawn_enemys)
        {
            yield return null; // Wait for the next frame
        }

        // Keep spawning while enabled
        while (isSpawning && LevelManager.Instance.spawn_enemys)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check if enemyPrefab is assigned before spawning
            if (enemyPrefab != null)
            {
                // Spawn the enemy at this GameObject's position and rotation
                GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
                isSpawning = false;

                // Play the spawn effect
                if (poofEffect != null)
                {
                    GameObject effectInstance = Instantiate(poofEffect,transform.position, transform.rotation);
                }

                // Find any ParticleSystem component on the spawned enemy or its children and destroy it
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Enemy prefab is not assigned!");
                isSpawning = false; // Stop spawning if no prefab is assigned
            }
        }
    }

    public void StopSpawning()
    {
        // Stop spawning enemies
        isSpawning = false;
    }
}
