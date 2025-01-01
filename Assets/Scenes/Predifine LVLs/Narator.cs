using System;
using System.Collections.Generic;
using UnityEngine;

public class Narator : MonoBehaviour
{
    public static Narator Instance { get; private set; }

    // Event system for narration signals
    public event Action<string> OnNarrationSignal;

    // Multiple audio clips and sources for narration
    public List<AudioClip> audioClips; // Assign clips in the Inspector
    public AudioSource primaryAudioSource; // Assign in the Inspector
    public AudioSource backgroundMusicSource; // Assign in the Inspector
    private Dictionary<string, AudioClip> clipDictionary;

    private float originalBackgroundVolume;

    
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
         if (primaryAudioSource == null || backgroundMusicSource == null)
        {
            Debug.LogError("Audio Sources not assigned in the Inspector!");
            return;
        }

        originalBackgroundVolume = backgroundMusicSource.volume;

        // Initialize clip dictionary for quick access by name
        clipDictionary = new Dictionary<string, AudioClip>();
        foreach (var clip in audioClips)
        {
            clipDictionary[clip.name] = clip;
        }
    }

    // Method to trigger narration based on a signal
    public void TriggerNarration(string signal)
    {
        if (clipDictionary.ContainsKey(signal))
        {
            Debug.Log($"play clip: {signal}");
            primaryAudioSource.clip = clipDictionary[signal];
            primaryAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"No audio clip found for signal: {signal}");
        }
    }

    void Update()
    {
        if (primaryAudioSource.isPlaying)
        {
            backgroundMusicSource.volume = originalBackgroundVolume * 0.1f; // Reduce volume by 50%
        }
        else
        {
            backgroundMusicSource.volume = originalBackgroundVolume; // Restore original volume
        }
    }

    public bool chekNaratorPlaying(string current_clip_name, string new_clip){
        if(primaryAudioSource.isPlaying && primaryAudioSource.clip == clipDictionary[current_clip_name])
        {
             primaryAudioSource.clip = clipDictionary[new_clip];
             primaryAudioSource.Play(); 
             return true;
        }
        return false;         
    }

    // Method to allow other scripts to subscribe to narration events
    public void RegisterListener(Action<string> listener)
    {
        OnNarrationSignal += listener;
    }

    // Method to unsubscribe listeners
    public void UnregisterListener(Action<string> listener)
    {
        OnNarrationSignal -= listener;
    }

    // External call for triggering a narration event
    public void SendNarrationSignal(string signal)
    {
        OnNarrationSignal?.Invoke(signal); // Notify all listeners
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
