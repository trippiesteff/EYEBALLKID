using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 0.5f;

    [Header("State")]
    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isRespawning = false;
    [SerializeField] private bool isTransitioning = false;

    private Coroutine respawnRoutine;
    private CinemachineImpulseSource impulseSource;

    public bool IsPaused => isPaused;
    public bool IsRespawning => isRespawning;
    public bool IsTransitioning => isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isPaused = false;
        isRespawning = false;
        isTransitioning = false;
        respawnRoutine = null;
    }

    public void FreezeFrame(float duration)
    {
        StartCoroutine(FreezeFrameCo(duration));
    }

    private IEnumerator FreezeFrameCo(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);

        if (!isPaused)
            Time.timeScale = 1f;
    }

    public void ScreenShake(float duration, float intensity)
    {
        if (impulseSource != null)
            impulseSource.GenerateImpulse(intensity);
    }

    public void PauseGame()
    {
        if (isRespawning || isTransitioning)
            return;

        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void RespawnAtLastCheckpoint()
    {
        if (isRespawning || isTransitioning)
            return;

        if (respawnRoutine != null)
            StopCoroutine(respawnRoutine);

        respawnRoutine = StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;
        isTransitioning = true;
        Time.timeScale = 1f;

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeOut();

        if (respawnDelay > 0f)
            yield return new WaitForSecondsRealtime(respawnDelay);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void RestartCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}