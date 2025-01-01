using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : MonoBehaviour
{
    public GameObject bombPrefab;
    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         // Create a bomb (sphere) in front of the capsule when pressing the "Z" key
        if (Input.GetKeyDown(KeyCode.Z) && active)
        {
            SpawnBomb();
        }
    }

    void SpawnBomb()
    {
         if (bombPrefab != null)
        {
            // Use the bomb to spawn at a specific point "spawnPosition"
            Vector3 spawnPosition = transform.position + transform.forward * 2;

            // Instantiate the bombPrefab at the given spawnPosition and with the default rotation
            Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

            Debug.Log("Bomb Spawned!");
        }
        else
        {
            Debug.LogError("Bomb prefab is not assigned!");
        }
    }

    public void setActive(){
        active = true;
    }
}
