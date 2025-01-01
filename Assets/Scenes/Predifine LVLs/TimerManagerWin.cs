using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{

    public float levelDuration = 120f;  // Set this per level in the prefab
    private float remainingTime;
    private bool isTimerRunning = false;

    // Events for notifying when the timer updates and expires
    public event Action<float> OnTimeChanged;
    public event Action OnTimeExpired;
    public event Action onLevelWin;
    public event Action onLevelLose;

    public bool isWin = true; // what condition to send regarding the wining condition at the end of the counter 


    private void Awake()
    {
        // // Set up the Singleton pattern
        // if (Instance == null)
        // {
        //     Instance = this;
        // }
        // else
        // {
        //     Destroy(gameObject); // Ensure there is only one TimerManager
        //     return;
        // }

        // Optionally, prevent this object from being destroyed between scenes
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // StartTimer(levelDuration);  // Start the timer using the level-specific duration
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            remainingTime -= Time.deltaTime;

            // Notify observers/UI about the remaining time
            OnTimeChanged?.Invoke(remainingTime);

            if (remainingTime <= 0)
            {
                TimerExpired();
            }
        }
    }

    // Start the timer with the specified duration
    public void StartTimer()
    {
        MainUI.Instance.startTimer(); // reset the timer
        remainingTime = levelDuration;
        isTimerRunning = true;
    }

    // Stop the timer
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // Handle when the timer expires
    private void TimerExpired()
    {
        isTimerRunning = false;
        if(isWin)
            onLevelWin?.Invoke();  // Notify that the time has expired
        else
            onLevelLose?.Invoke(); 
        Destroy(this); // destroy the commponent    

    }
}
