using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeController : MonoBehaviour
{
    // Static instance of LifeController to allow other scripts to access it easily
    public static LifeController instance;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Assign this instance to the static instance variable
        instance = this;
    }

    // Reference to the NewPlayerController script
    private NewPlayerController thePlayer;

    // Delay before respawning the player
    public float respawnDelay = 2f;

    // Number of lives the player currently has
    public int currentLives = 3;

    // GameObjects for death and respawn effects
    public GameObject deathEffect, respawnEffect;

    // Start is called before the first frame update
    void Start()
    {
        // Find the first object of type NewPlayerController and assign it to thePlayer
        thePlayer = FindFirstObjectByType<NewPlayerController>();

        // Get the current lives from the InfoTracker instance
        currentLives = InfoTracker.instance.currentLives;

        // Update the display with the current number of lives
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        // Currently empty; can be used for updates per frame if needed
    }

    // Method to handle player respawn
    public void Respawn()
    {
        // Disable the player's GameObject
        thePlayer.gameObject.SetActive(false);

        // Reset the player's velocity
        thePlayer.theRB.velocity = Vector2.zero;

        // Decrease the current lives by 1
        // currentLives--;

        // Check if there are remaining lives
        if (currentLives > 0)
        {
            // Start the respawn coroutine
            StartCoroutine(RespawnCo());
        }
        else
        {
            // Set current lives to 0 and start the game over coroutine
            currentLives = 0;
            StartCoroutine(GameOverCo());
        }

        // Update the display with the current number of lives
        UpdateDisplay();

        // Instantiate the death effect at the player's position
        Instantiate(deathEffect, thePlayer.transform.position, deathEffect.transform.rotation);

        // Play the death sound effect
        AudioManager.Instance.PlaySFX(11);
    }

    // Coroutine for handling respawn with delay
    public IEnumerator RespawnCo()
    {
        // Wait for the respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Set the player's position to the respawn position
        thePlayer.transform.position = FindFirstObjectByType<CheckpointManager>().respawnPosition;

        // Restore the player's health to max
        PlayerHealthController.instance.AddHealth(PlayerHealthController.instance.maxHealth);

        // Enable the player's GameObject
        thePlayer.gameObject.SetActive(true);

        // Instantiate the respawn effect at the player's position
        Instantiate(respawnEffect, thePlayer.transform.position, Quaternion.identity);
    }

    // Coroutine for handling game over with delay
    public IEnumerator GameOverCo()
    {
        // Wait for the respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Show the game over UI if UIController instance is not null
        if(UIController.instance != null)
        {
            UIController.instance.ShowGameOver();
        }
    }

    // Method to add a life to the player
    public void AddLife()
    {
        // Code to increase lives and update display is commented out
        // currentLives++;

        // Update the display with the current number of lives
        // UpdateDisplay();

        // Play the life gain sound effect
        // AudioManager.Instance.PlaySFX(8);
    }

    // Method to update the UI display with the current number of lives
    public void UpdateDisplay()
    {
        // Update the lives display on the UI if UIController instance is not null
        if (UIController.instance != null)
        {
            UIController.instance.UpdateLivesDisplay(currentLives);
        }
    }
}
