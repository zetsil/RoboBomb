using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionString = System.Action<string>;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; } // Singleton instance

    public List<GameObject> predefinedLevels; // This will be a list of prefabs
    public Transform startPoint;
    private Transform lastLevelEndPoint;// last_level_start_point will remmber the end of the level
    public int predifine_lvl_count = 0; // this will keep in mind what predifined level was drawn from the list

    private TimerManager activeTimerManager;
    private GameObject prevLvl;
    public event ActionString OnNarrationSignal;

    private int current_lvl_index = 0;
    public bool spawn_enemys = true;
    private GameObject currentLevel;

    public bool useCheckPoint = false;




    // Start is called before the first frame update
    void Start()
    {
        OnNarrationSignal = Narator.Instance.TriggerNarration; // subscribe to narator function
        // Check if the startPoint is set
        if (startPoint == null)
        {
            Debug.LogWarning("Start Point is not set! Please assign a start point in the inspector.");
        }
        else
        {
            // Optional: You can proceed to initialize the level using startPoint
            Debug.Log("Start level 1");
            int checkpoint = PersistentData.Instance.gameData.lastCheckpointIndex; //load from checkpoint 
            if(checkpoint != -1 && useCheckPoint) 
                predifine_lvl_count = checkpoint; //load from checkpoint 
            current_lvl_index = predifine_lvl_count;
            InitializePredifineLevel(predifine_lvl_count, startPoint);
            prevLvl = currentLevel;
            StartTimer();
            if(current_lvl_index == 0)
                OnNarrationSignal?.Invoke("start_game_glados");
        }
    }

    void startAtChepoint(int checkpointIndex, Transform start)
    {
        InitializePredifineLevel(checkpointIndex,startPoint);
    }

    private void Awake()
    {
        // Check if an instance already exists in the scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate instances
        }
        else
        {
            Instance = this; // Set the singleton instance
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nextLevel(int index){
        current_lvl_index = index;
        InitializePredifineLevel(index, lastLevelEndPoint);
    }

    public void StartTimer(){
        activeTimerManager.StartTimer();
    }

    public void nextLevel(){ // this is basicaly called when win condition is happening

      if (lastLevelEndPoint == null)
        {
            return;
        }
        current_lvl_index++;
        activeTimerManager.StopTimer();// stop the timer until player reach other platform
        spawn_enemys = false; //stop the spawn enemys until player reach other platform
        prevLvl = currentLevel; // rember the prev platform lvl
        InitializePredifineLevel(current_lvl_index, lastLevelEndPoint);
    }

    public void DestroyExistingLevels()
    {
        // Find all instantiated level objects (you might need to adjust this based on how you parent them)
        GameObject[] levelObjects = GameObject.FindGameObjectsWithTag("Level"); // Or any other suitable tag

        foreach (GameObject level in levelObjects)
        {
            Destroy(level);
        }
    }

    void InitializePredifineLevel(int index, Transform start)
    {
        if (index >= 0 && index < predefinedLevels.Count)
        {
            // Instantiate the level prefab at the startPoint's position and rotation
            GameObject level = Instantiate(predefinedLevels[index], start.position, start.rotation);
            currentLevel = level;
            // currentLevel.tag = "Level"; // Add a tag for easy finding later
            Debug.Log("Initialized level: " + predefinedLevels[index].name + " at position: " + start.position);
            EnemyManager.Instance.CountEnemies();
            // Get the TimerManager from the instantiated level
            activeTimerManager = level.GetComponent<TimerManager>();

            // Ensure the timer is set for the level and start it
            if (activeTimerManager != null)
            {
                activeTimerManager.onLevelWin += nextLevel;
                activeTimerManager.onLevelLose += CollapseCurrentLvl;
            }

            // Calculate the last level end point based on the EndLvlCollider
            lastLevelEndPoint = FindEndLevelCollider(level.transform);
            
            // Set the Y position of the last level end point
            Vector3 newPosition = lastLevelEndPoint.transform.position; // Get the current position
            newPosition.y = level.transform.position.y; // Set the desired Y value
            newPosition.z = level.transform.position.z; // Set the desired Y value

            // Create a new GameObject
            GameObject newTransformObject = new GameObject("NewTransformObject");

            // Set the position of the new GameObject's transform to the new coordinates
            newTransformObject.transform.position = newPosition;
            
            lastLevelEndPoint = newTransformObject.transform; // Update the position

            if (lastLevelEndPoint != null)
            {
                Debug.Log("Last level end point set to: " + lastLevelEndPoint.position);
                // increase the level count for next use 
                predifine_lvl_count++;
            }
            else
            {
                Debug.LogWarning("No EndLvlCollider found in the instantiated level prefab.");
            }
        }
        else
        {
            Debug.Log("You have finished all the levels!");
        }
    }

    private Transform FindEndLevelCollider(Transform parent)
    {
        // Search for the child with the tag "EndLvlCollider"
        foreach (Transform child in parent)
        {
            if (child.CompareTag("EndLvlPoint"))
            {
                return child; // Return the transform of the EndLvlCollider
            }
        }

        // Return null if not found
        return null;
    }


    private void UpdateTimerUI(float remainingTime)
        {
            // Debug.Log("Remaining Time: " + remainingTime);
            // Update the UI here (you can also link this to your UIManager)
        }

    private void HandleTimeExpired()
        {
            Debug.Log("Time's up! Handle level failure or apply a penalty.");
            // Collapse(prevLvl);
            // Handle time expiration logic, like failing the level or reducing player health, etc.
        }
    public void CollapsePrevLvl(){
        Collapse(prevLvl);
    }    

    private void CollapseCurrentLvl(){
        Collapse(currentLevel);
    }

    void Collapse(GameObject obj)
    {
        // Add Rigidbody and apply force/torque to the current object (root or child)
        AddRigidbodyAndForce(obj);
        Destroy(obj,5f);

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
        Vector3 downwardDirection = new Vector3(
            Random.Range(-0.5f, 0.5f),  // Small random variation on the X-axis
            -1f,                         // Strong downward force on the Y-axis
            Random.Range(-0.5f, 0.5f)   // Small random variation on the Z-axis
        ).normalized;

        // Apply the force with an upward bias
        rb.AddForce(downwardDirection * Random.Range(10f, 100f)); // Adjust the force range as needed

        // Optionally: Add some random torque to make parts spin
        rb.AddTorque(Random.insideUnitSphere * Random.Range(10f, 50f));
    }   
}
