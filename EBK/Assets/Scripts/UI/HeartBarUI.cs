using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartBarUI : MonoBehaviour
{
    [Header("References")]
    public Entity_Health playerHealth;
    public GameObject heartTemplate;
    public string heartImageChildName = "HeartImage";

    [Header("Sprites")]
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("Low Health Warning")]
    public int lowHealthThreshold = 2;
    public Color normalColor = Color.white;
    public Color lowHealthBlinkColor = new Color(1f, 0.45f, 0.45f, 1f);
    public float blinkSpeed = 4f;

    [Header("Idle Wiggle")]
    public float wiggleAmountX = 0.25f;
    public float wiggleAmountY = 0.75f;
    public float wiggleSpeed = 2f;

    private List<GameObject> heartSlots = new List<GameObject>();
    private List<Image> heartImages = new List<Image>();
    private List<RectTransform> heartImageRects = new List<RectTransform>();
    private List<Vector2> basePositions = new List<Vector2>();

    private void Start()
    {
        if (heartTemplate != null)
            heartTemplate.SetActive(false);

        RedrawHearts();
    }

    private void Update()
    {
        RedrawHearts();
        AnimateHearts();
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

        bool lowHealth = currentHealth <= lowHealthThreshold;
        Color blinkColor = lowHealth
            ? Color.Lerp(normalColor, lowHealthBlinkColor, Mathf.PingPong(Time.time * blinkSpeed, 1f))
            : normalColor;

        for (int i = 0; i < heartSlots.Count; i++)
        {
            if (i < maxHealth)
            {
                heartSlots[i].SetActive(true);

                if (i < currentHealth)
                    heartImages[i].sprite = fullHeartSprite;
                else
                    heartImages[i].sprite = emptyHeartSprite;

                if (lowHealth && i < currentHealth)
                    heartImages[i].color = blinkColor;
                else
                    heartImages[i].color = normalColor;
            }
            else
            {
                heartSlots[i].SetActive(false);
            }
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
