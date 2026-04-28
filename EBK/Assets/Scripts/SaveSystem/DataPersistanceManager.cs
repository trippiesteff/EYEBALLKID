using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName = "savegame.json";
    [SerializeField] private bool useEncryption = false;

    public static DataPersistenceManager Instance { get; private set; }

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

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
        NewGame();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            NewGame();
        }

        dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
{
    if (gameData == null)
    {
        return;
    }

    foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
    {
        dataPersistenceObj.SaveData(gameData);
    }

    gameData.currentSceneName = SceneManager.GetActiveScene().name;
    gameData.lastUpdated = System.DateTime.Now.ToBinary();
    dataHandler.Save(gameData);
}


    public void SetPendingDestination(string destinationId)
    {
        if (gameData == null)
        {
            NewGame();
        }

        gameData.pendingDestinationId = destinationId;
    }

    public string GetPendingDestination()
    {
        if (gameData == null)
        {
            NewGame();
        }

        return gameData.pendingDestinationId;
    }

    public void ClearPendingDestination()
    {
        if (gameData == null)
        {
            NewGame();
        }

        gameData.pendingDestinationId = "";
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public string GetCurrentSceneNameFromSave()
{
    if (gameData == null)
    {
        gameData = dataHandler.Load();
    }

    if (gameData == null)
    {
        return "";
    }

    return gameData.currentSceneName;
}

}
