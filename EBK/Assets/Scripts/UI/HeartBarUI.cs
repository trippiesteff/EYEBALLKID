using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeartBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Entity_Health playerHealth;
    [SerializeField] private GameObject heartTemplate;
    [SerializeField] private string heartImageChildName = "HeartImage";

    [Header("Sprites")]
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Low Health Warning")]
    [SerializeField] private int lowHealthThreshold = 2;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowHealthBlinkColor = new Color(1f, 0.45f, 0.45f, 1f);
    [SerializeField] private float blinkSpeed = 4f;

    [Header("Idle Wiggle")]
    [SerializeField] private float wiggleAmountX = 0.25f;
    [SerializeField] private float wiggleAmountY = 0.75f;
    [SerializeField] private float wiggleSpeed = 2f;

    private readonly List<GameObject> heartSlots = new();
    private readonly List<Image> heartImages = new();
    private readonly List<RectTransform> heartImageRects = new();
    private readonly List<Vector2> basePositions = new();

    private int lastMaxHealth = -1;
    private int lastCurrentHealth = -1;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (heartTemplate != null)
            heartTemplate.SetActive(false);

        RefreshPlayerReference();
        ForceRebuild();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshPlayerReference();
        ForceRebuild();
    }

    private void Update()
    {
        if (playerHealth == null)
        {
            RefreshPlayerReference();
            return;
        }

        int maxHealth = Mathf.RoundToInt(playerHealth.GetMaxHealth());
        int currentHealth = Mathf.RoundToInt(playerHealth.GetCurrentHealth());

        if (maxHealth != lastMaxHealth || currentHealth != lastCurrentHealth)
        {
            RedrawHearts();
            lastMaxHealth = maxHealth;
            lastCurrentHealth = currentHealth;
        }

        AnimateHearts();
    }

    private void RefreshPlayerReference()
    {
        Player player = FindFirstObjectByType<Player>();

        if (player != null)
            playerHealth = player.GetComponent<Entity_Health>();
    }

    public void ForceRebuild()
    {
        ClearHearts();

        if (playerHealth == null)
            return;

        RedrawHearts();
        lastMaxHealth = Mathf.RoundToInt(playerHealth.GetMaxHealth());
        lastCurrentHealth = Mathf.RoundToInt(playerHealth.GetCurrentHealth());
    }

    private void ClearHearts()
    {
        foreach (GameObject slot in heartSlots)
        {
            if (slot != null)
                Destroy(slot);
        }

        heartSlots.Clear();
        heartImages.Clear();
        heartImageRects.Clear();
        basePositions.Clear();
    }

    private void RedrawHearts()
    {
        if (playerHealth == null || heartTemplate == null)
            return;

        int maxHealth = Mathf.RoundToInt(playerHealth.GetMaxHealth());
        int currentHealth = Mathf.RoundToInt(playerHealth.GetCurrentHealth());

        while (heartSlots.Count < maxHealth)
        {
            GameObject newSlot = Instantiate(heartTemplate, transform);
            newSlot.SetActive(true);
            heartSlots.Add(newSlot);

            Transform imageChild = newSlot.transform.Find(heartImageChildName);
            if (imageChild == null)
            {
                Debug.LogError("HeartImage child not found on heart template.");
                return;
            }

            Image image = imageChild.GetComponent<Image>();
            RectTransform rect = imageChild.GetComponent<RectTransform>();

            heartImages.Add(image);
            heartImageRects.Add(rect);
            basePositions.Add(rect.anchoredPosition);
        }

        for (int i = 0; i < heartSlots.Count; i++)
        {
            heartSlots[i].SetActive(i < maxHealth);
        }

        bool lowHealth = currentHealth <= lowHealthThreshold;
        Color blinkColor = lowHealth
            ? Color.Lerp(normalColor, lowHealthBlinkColor, Mathf.PingPong(Time.time * blinkSpeed, 1f))
            : normalColor;

        for (int i = 0; i < maxHealth; i++)
        {
            heartImages[i].sprite = i < currentHealth ? fullHeartSprite : emptyHeartSprite;
            heartImages[i].color = (lowHealth && i < currentHealth) ? blinkColor : normalColor;
            heartImageRects[i].anchoredPosition = basePositions[i];
        }
    }

    private void AnimateHearts()
    {
        for (int i = 0; i < heartImageRects.Count; i++)
        {
            if (!heartSlots[i].activeSelf)
                continue;

            float timeOffset = i * 0.35f;
            float offsetX = Mathf.Sin(Time.time * wiggleSpeed + timeOffset) * wiggleAmountX;
            float offsetY = Mathf.Cos(Time.time * wiggleSpeed + timeOffset) * wiggleAmountY;

            heartImageRects[i].anchoredPosition = basePositions[i] + new Vector2(offsetX, offsetY);
        }
    }
}
