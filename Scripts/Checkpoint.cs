using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// Class to manage individual checkpoints
public class Checkpoint : MonoBehaviour
{
    // Boolean to check if the checkpoint is active
    private bool isActive;
    // Animator to handle checkpoint animations
    public Animator anim;
    // Reference to the CheckpointManager
    [HideInInspector]
    public CheckpointManager cpMan;

  // Called when another collider enters the trigger collider attached to this object
  private void OnTriggerEnter2D(Collider2D other)
  {
    // Check if the collider belongs to the player and the checkpoint is not already active
    if(other.tag == "Player" && isActive == false)
    {
        // Set this checkpoint as the active checkpoint
        cpMan.SetActiveCheckpoint(this);

        // Activate the checkpoint animation
        anim.SetBool("flagActive", true);

        // Set the checkpoint as active
        isActive = true;

        AudioManager.Instance.PlaySFX(3);
    }
  }
   // Deactivate the checkpoint
   public void DeactivateCheckpoint()
    {
        anim.SetBool("flagActive", false);
        isActive = false;
    }
}
