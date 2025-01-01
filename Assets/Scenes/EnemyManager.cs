using System.Collections.Generic;
using UnityEngine;
using MyGameNamespace; // Import the namespace containing AtackBot
using System;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    private int totalEnemies;
    private int aliveEnemies;
    private int totalSpawners;
    private List<AtackBot> enemyList = new List<AtackBot>();
    public event Action onLevelWin;



     private void Awake()
    {
        // Ensure this is the only instance (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional if you want it to persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }    

    private void Start()
    {
        InitializeEnemies();
        this.onLevelWin += LevelManager.Instance.nextLevel;
    }

    public void OnDisable() // Or OnDestroy, depending on your needs
    {
        this.onLevelWin -= LevelManager.Instance.nextLevel;
    }

    private void InitializeEnemies()
    {

        // Find all objects tagged "SpawnEnemy" in the scene
        GameObject[] spawnerObjects = GameObject.FindGameObjectsWithTag("SpawnEnemy");
        
        // Count the spawners and store them in an array
        totalSpawners = spawnerObjects.Length;
        // spawners = new SpawnEnemy[totalSpawners];
        aliveEnemies = totalSpawners;

        Debug.Log("Total Enemy Spawners: " + totalSpawners);
    }

    public void CountEnemies()
    {
        // Find all objects tagged "SpawnEnemy" in the scene
        GameObject[] spawnerObjects = GameObject.FindGameObjectsWithTag("SpawnEnemy");
        
        // Count the spawners and store them in an array
        totalSpawners = spawnerObjects.Length;
        // spawners = new SpawnEnemy[totalSpawners];
        aliveEnemies = totalSpawners;

        Debug.Log("Total Enemy Spawners: " + totalSpawners);
    }

    public void HandleEnemyDeath()
    {
        aliveEnemies--;
        Debug.Log("Enemy defeated. Remaining enemies: " + aliveEnemies);

        // Additional logic if needed, such as checking if all enemies are defeated
        if (aliveEnemies <= 0)
        {
            Debug.Log("All enemies defeated. Level cleared!");
            onLevelWin?.Invoke();  // Notify that player cleared the room

        }
    }
}
