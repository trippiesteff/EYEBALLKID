using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    // Singleton instance of the PlayerHealthController
    public static PlayerHealthController instance;

    private void Awake()
    {
        // Assign the instance to this object
        instance = this;
    }

    public int currentHealth, maxHealth;
    public float invincibilityLength = 1f;
    private float invincibilityCounter;
    public SpriteRenderer theSR;
    public Color normalColor, fadeColor;
    private NewPlayerController thePlayer; // Change this to NewPlayerController

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = GetComponent<NewPlayerController>(); // Ensure this gets NewPlayerController
        currentHealth = maxHealth;
        UIController.instance.UpdateHealthDisplay(currentHealth, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
            if (invincibilityCounter <= 0)
            {
                theSR.color = normalColor;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            AddHealth(1);
        }
#endif
    }

    public void DamagePlayer()
    {
        if (invincibilityCounter <= 0)
        {
            currentHealth--;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                LifeController.instance.Respawn();
                AudioManager.Instance.PlaySFX(11);
            }
            else
            {
                invincibilityCounter = invincibilityLength;
                theSR.color = fadeColor;
                thePlayer.KnockBack(); // Ensure this calls KnockBack on NewPlayerController
                AudioManager.Instance.PlaySFX(13);
            }
            UIController.instance.UpdateHealthDisplay(currentHealth, maxHealth);
        }
    }

    public void AddHealth(int amountToAdd)
    {
        currentHealth += amountToAdd;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UIController.instance.UpdateHealthDisplay(currentHealth, maxHealth);
    }
}
