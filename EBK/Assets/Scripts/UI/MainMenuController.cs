using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string firstGameSceneName = "Level_01";

    public void StartNewGame()
    {
        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.NewGame();
            DataPersistenceManager.Instance.SaveGame();
        }

        SceneManager.LoadScene(firstGameSceneName);
    }

    public void ContinueGame()
    {
        string sceneToLoad = firstGameSceneName;

        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.LoadGame();

            string savedSceneName = DataPersistenceManager.Instance.GetCurrentSceneNameFromSave();

            if (string.IsNullOrEmpty(savedSceneName) == false)
            {
                sceneToLoad = savedSceneName;
            }
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
