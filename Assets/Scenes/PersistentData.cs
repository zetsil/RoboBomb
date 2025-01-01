using UnityEngine;
using System;
using System.IO;

[Serializable]
public class GameData
{
    public int lastCheckpointIndex;
    // public Vector3 lastCheckpointPosition;
    // public PlayerData playerData;

    public GameData()
    {
        lastCheckpointIndex = -1; // -1 indicates no checkpoint reached yet
        // lastCheckpointPosition = Vector3.zero;
        // playerData = new PlayerData(); // Initialize PlayerData
    }
}

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance { get; private set; }

    public GameData gameData;

    private string saveFilePath;

    private void Awake()
    {
        // Singleton pattern:
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes

        saveFilePath = Application.persistentDataPath + "/gameData.json";
        LoadGameData();
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadGameData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData(); // Create new data if no file exists
        }
    }

    public void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            // Delete the save file
            File.Delete(saveFilePath);

            // Reset the game data object
            gameData = new GameData();

        }
        else
        {
            Debug.Log("No save data found to delete.");
        }
    }

    public void SetCheckpoint(int checkpointIndex)
    {
        gameData.lastCheckpointIndex = checkpointIndex;
        // gameData.lastCheckpointPosition = checkpointPosition;
        // gameData.playerData.health = player.health;
        // gameData.playerData.score = player.score;
        // gameData.playerData.level = player.level;
        // gameData.playerData.time = TimerManager.Instance.GetCurrentTime();
        // gameData.playerData.lastNarrationPlayed = Narator.Instance.lastNarrationPlayed;
        SaveGameData();
    }
}