using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName, string destinationId)
    {
        if (isTransitioning)
            return;

        StartCoroutine(LoadSceneRoutine(sceneName, destinationId));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, string destinationId)
    {
        isTransitioning = true;

        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.SetPendingDestination(destinationId);
            DataPersistenceManager.Instance.SaveGame();
        }

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeOut();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
            yield return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(OnSceneLoadedRoutine());
    }

    private IEnumerator OnSceneLoadedRoutine()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        string pendingDestination = DataPersistenceManager.Instance != null
            ? DataPersistenceManager.Instance.GetPendingDestination()
            : "";

        if (!string.IsNullOrEmpty(pendingDestination))
        {
            SceneTransitionDestination[] destinations = FindObjectsByType<SceneTransitionDestination>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (SceneTransitionDestination destination in destinations)
            {
                if (destination.DestinationId != pendingDestination)
                    continue;

                PlayerData player = FindFirstObjectByType<PlayerData>();

                if (player != null)
                    player.transform.position = destination.transform.position;

                if (DataPersistenceManager.Instance != null)
                {
                    DataPersistenceManager.Instance.ClearPendingDestination();
                    DataPersistenceManager.Instance.SaveGame();
                }

                break;
            }
        }

        yield return null;
        yield return new WaitForEndOfFrame();

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeIn();

        isTransitioning = false;
    }
}
