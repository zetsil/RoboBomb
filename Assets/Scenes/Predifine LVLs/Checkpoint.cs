using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                PersistentData.Instance.SetCheckpoint(checkpointIndex);
                // LevelManager.Instance.RebuildLevelFromCheckpoint(checkpointIndex, transform);
            
        }
    }
}