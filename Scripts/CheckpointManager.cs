using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to manage all checkpoints in the game
public class CheckpointManager : MonoBehaviour
{
    // Array to store all checkpoints in the scene
    public Checkpoint[] allCP;
    // The currently active checkpoint
    private Checkpoint activeCP;
    public Vector3 respawnPosition;

    void Start()
    {
        // Find all checkpoint objects in the scene
        allCP = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        // Assign this manager to each checkpoint
        foreach(Checkpoint cp in allCP)
        {
            cp.cpMan = this;
        }

        respawnPosition = FindFirstObjectByType<NewPlayerController>().transform.position;
    }

    void Update()
    {
        
    }

    // Deactivate all checkpoints
    public void DeactivateAllCheckpoints()
    {
        foreach(Checkpoint cp in allCP)
        {
            cp.DeactivateCheckpoint();
        }
    }

    // Set a new active checkpoint and deactivate the rest
    public void SetActiveCheckpoint(Checkpoint newActiveCP)
    {
        DeactivateAllCheckpoints();
        activeCP = newActiveCP;
        respawnPosition = newActiveCP.transform.position;
    }
}
