using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private bool fadeInOnStart = true;

    private Coroutine currentRoutine;
    private Transform persistentRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject);
            return;
        }

        Instance = this;
        persistentRoot = transform.root;
        DontDestroyOnLoad(persistentRoot.gameObject);

        SetAlpha(1f);
    }

    private void Start()
    {
        if (fadeInOnStart)
            FadeIn();
    }

    public Coroutine FadeOut()
    {
        return FadeTo(1f);
    }

    public Coroutine FadeIn()
    {
        return FadeTo(0f);
    }

    private Coroutine FadeTo(float targetAlpha)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeRoutine(targetAlpha));
        return currentRoutine;
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        if (fadeImage == null)
            yield break;

        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
        currentRoutine = null;
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage == null)
            return;

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
