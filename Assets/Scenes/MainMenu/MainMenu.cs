using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the from last chekpoint level 
        SceneManager.LoadScene(1); // Assuming your first level is at index 1 in the build settings
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }

    public void NewGame()
    {
        PersistentData.Instance.DeleteSaveData();
        SceneManager.LoadScene(1); // Load the first level
    }
}