using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// Class to manage the user interface elements
public class UIController : MonoBehaviour
{
    // Singleton instance of the UIController
    public static UIController instance;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Assign this instance to the static instance variable
        instance = this;  
    }

    // Array of Image components representing heart icons
    public Image[] heartIcons;

    // Sprites for full and empty heart icons
    public Sprite heartFull, heartEmpty;

    // Text component to display the number of lives
    public TMP_Text livesText;

    // Text component to display the number of collectibles
    public TMP_Text collectiblesText;

    // GameObjects for the game over screen and pause screen
    public GameObject gameOverScreen, pauseScreen;

    // Name of the main menu scene to load
    public string mainMenuScene;

    // Image component for screen fade effect
    public Image fadeScreen;

    // Speed of the fade effect
    public float fadeSpeed;

    // Booleans to control fading to and from black
    public bool fadingToBlack, fadingFromBlack;

    // GameObjects to be selected first when pause or game over screens are shown
    public GameObject pauseMenuFirst, gameOverScreenFirst;

    // Start is called before the first frame update
    void Start()
    {
        // Set the time scale to normal speed
        Time.timeScale = 1f;

        // Start fading from black
        FadeFromBlack();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the cancel button (usually Escape) is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            // Toggle the pause menu
            PauseUnpause();
        }

        // Handle fading from black
        if (fadingFromBlack)
        {
            // Gradually decrease the alpha value of the fade screen color
            fadeScreen.color = new Color(
                fadeScreen.color.r,
                fadeScreen.color.g,
                fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
        }

        // Handle fading to black
        if (fadingToBlack)
        {
            // Gradually increase the alpha value of the fade screen color
            fadeScreen.color = new Color(
                fadeScreen.color.r,
                fadeScreen.color.g,
                fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
        }
    }

    // Method to update the health display
    public void UpdateHealthDisplay(int health, int maxHealth)
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            // Enable each heart icon
            heartIcons[i].enabled = true;

            // Set the heart icon to full or empty based on the current health
            if (health > i)
            {
                heartIcons[i].sprite = heartFull;
            }
            else
            {
                heartIcons[i].sprite = heartEmpty;

                // Disable the heart icon if it exceeds the max health
                if (maxHealth <= i)
                {
                    heartIcons[i].enabled = false;
                }
            }
        }
    }

    // Method to update the lives display
    public void UpdateLivesDisplay(int currentLives)
    {
        // Set the lives text to the current number of lives
        livesText.text = currentLives.ToString();
    }

    // Method to show the game over screen
    public void ShowGameOver()
    {
        // Activate the game over screen
        gameOverScreen.SetActive(true);

        // Set the first selected GameObject for the event system
        EventSystem.current.SetSelectedGameObject(gameOverScreenFirst);
    }

    // Method to restart the current scene
    public void Restart()
    {
        // Load the current scene again
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Set the time scale to normal speed
        Time.timeScale = 1f;
    }

    // Method to update the collectibles display
    public void UpdateCollectibles(int amount)
    {
        // Set the collectibles text to the current amount
        collectiblesText.text = amount.ToString();
    }

    // Method to toggle the pause screen
    public void PauseUnpause()
    {
        if (pauseScreen.activeSelf == false)
        {
            // Activate the pause screen and set the time scale to 0 (pause)
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;

            // Set the first selected GameObject for the event system
            EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
        }
        else
        {
            // Deactivate the pause screen and set the time scale to normal speed
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // Method to load the main menu scene
    public void MainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuScene);
    }

    // Method to quit the game
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();

        // Log a message to the console (for debugging purposes)
        Debug.Log("I Quit");
    }

    // Method to start fading from black
    public void FadeFromBlack()
    {
        // Set the fading flags
        fadingToBlack = false;
        fadingFromBlack = true;
    }

    // Method to start fading to black
    public void FadeToBlack()
    {
        // Set the fading flags
        fadingToBlack = true;
        fadingFromBlack = false;
    }
}
