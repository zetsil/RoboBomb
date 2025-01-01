using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_Script : MonoBehaviour
{
    public static bool restartGame = false;
    public GameObject gameOverUI;
    public GameObject plainGamePrefab;
    public Button restartButton;
    public GameObject player;
    private Transform original_position;

    [SerializeField] private GameObject pausePanel; // Reference to the pause panel

    private bool isPaused = false;

    void Start()
    {
        if (restartButton == null)
        {
            restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        }
        original_position = player.transform;
        gameOverUI.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause panel not assigned in the Inspector!");
        }
    }

    private void Update()
    {
        if (restartGame)
        {
            GameOver();
            restartGame = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        restartButton.interactable = true;
        EnemyManager.Instance.OnDisable();
        LevelManager.Instance.DestroyExistingLevels();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}