using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject backgroundImage;
    public GameObject topTabs;

    public GameObject menuPage;
    public GameObject inventoryPage;
    public GameObject settingsPage;

    private bool isOpen;

    private void Start()
    {
        backgroundImage.SetActive(false);
        topTabs.SetActive(false);

        menuPage.SetActive(false);
        inventoryPage.SetActive(false);
        settingsPage.SetActive(false);

        Time.timeScale = 1f;
        isOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
                CloseMenu();
            else
                OpenInventoryPage();
        }
    }

    public void OpenMenuPage()
    {
        backgroundImage.SetActive(true);
        topTabs.SetActive(true);

        menuPage.SetActive(true);
        inventoryPage.SetActive(false);
        settingsPage.SetActive(false);

        Time.timeScale = 0f;
        isOpen = true;
    }

    public void OpenInventoryPage()
    {
        backgroundImage.SetActive(true);
        topTabs.SetActive(true);

        menuPage.SetActive(false);
        inventoryPage.SetActive(true);
        settingsPage.SetActive(false);

        Time.timeScale = 0f;
        isOpen = true;
    }

    public void OpenSettingsPage()
    {
        backgroundImage.SetActive(true);
        topTabs.SetActive(true);

        menuPage.SetActive(false);
        inventoryPage.SetActive(false);
        settingsPage.SetActive(true);

        Time.timeScale = 0f;
        isOpen = true;
    }

    public void CloseMenu()
    {
        backgroundImage.SetActive(false);
        topTabs.SetActive(false);

        menuPage.SetActive(false);
        inventoryPage.SetActive(false);
        settingsPage.SetActive(false);

        Time.timeScale = 1f;
        isOpen = false;
    }
}
