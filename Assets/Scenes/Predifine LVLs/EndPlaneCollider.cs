using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionString = System.Action<string>;

public class EndPlaneCollider : MonoBehaviour
{
    private bool playerWin = false;
    private TimerManager timeManager;
    public ActionString OnNarrationSignal;

    private bool endColider = false;
    public string play_sound_string = null; // used to play a sound when exit the collider

    void Awake(){
        OnNarrationSignal += Narator.Instance.TriggerNarration;
    }
    private void Start()
    {
        timeManager = gameObject.GetComponent<TimerManager>();
        // Subscribe to the TimerManager's OnTimeExpired event
        timeManager.onLevelWin += HandleWin;
        EnemyManager.Instance.onLevelWin += HandleWin;

    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (timeManager != null)
        {
            timeManager.OnTimeExpired -= HandleWin;
        }
        // if (OnNarrationSignal != null)
        //     OnNarrationSignal -= Narator.Instance.TriggerNarration;
    }

    // This will be called when the timer has expired
    private void HandleWin()
    {
        playerWin = true;
        Debug.Log("Timer expired. You can now leave the platform.");
    }

    // Check if the player enters the collider after time has expired
    private void OnTriggerEnter(Collider other)
    {
        if(endColider)
            return;
        if (other.CompareTag("Player"))
        {
            if (playerWin)
            {
                Debug.Log("Player win the current lvl time to exit!");
                LevelManager.Instance.CollapsePrevLvl();
                endColider = true;
                
            }
            else
            {
                Debug.Log("Player cannot exit yet. Timer is still running.");
            }
        }
    }

     private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerWin)
            {
                Debug.Log("Player exit colider!");

                if(play_sound_string != null)
                    OnNarrationSignal?.Invoke(play_sound_string);
                OnNarrationSignal -= Narator.Instance.TriggerNarration;
                LevelManager.Instance.spawn_enemys = true; // start spawing the enemys after when the player reach other platform
                LevelManager.Instance.StartTimer(); // start timer when the player reach other platform

            }
            else
            {
                Debug.Log("Player cannot exit yet");
            }
        }
    }

}