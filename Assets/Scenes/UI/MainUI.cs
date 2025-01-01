using TMPro;
using UnityEngine;
using System.Collections;

public class MainUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText; // Reference to the TextMeshProUGUI component
    private TimerManager timer;


       // Singleton instance
    public static MainUI Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if it's not the singleton
            return;
        }

        Instance = this; // Assign the singleton instance
        // DontDestroyOnLoad(gameObject); // Optional: Keep the UI alive through scene changes
    }

    public void startTimer() // Take  the new timer after 
    {
        timer = FindObjectOfType<TimerManager>();
        // Subscribe to the event once the Timer is found
        timer.OnTimeChanged += UpdateCounterText;
    }

    private void Start()
    {
        // StartCoroutine(WaitForTimer());
    }

    private IEnumerator WaitForTimer()
    {
        // Continuously check until a Timer instance is found
        while (timer == null)
        {
            timer = FindObjectOfType<TimerManager>();
            yield return null; // Wait one frame before checking again
        }

        // Subscribe to the event once the Timer is found
        timer.OnTimeChanged += UpdateCounterText;
    }


    private void OnDestroy()
    {
        // Unsubscribe from the event when this object is destroyed
        if (timer != null)
        {
            timer.OnTimeChanged -= UpdateCounterText;
        }
    }

    // Method to update UI text when time changes
    private void UpdateCounterText(float remainingTime)
    {
        counterText.text = $"Time: {remainingTime:F2}";
    }
}
