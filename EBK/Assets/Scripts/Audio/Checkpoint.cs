using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string checkpointId;
    [SerializeField] private bool isActive = false;

    private void Reset()
    {
        checkpointId = gameObject.name;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        ActivateCheckpoint();
    }

    private void ActivateCheckpoint()
    {
        isActive = true;

        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.SaveGame();
        }
    }

    public void LoadData(GameData data)
    {
        if (data == null)
            return;

        isActive = data.currentCheckpointId == checkpointId;

        if (isActive && data.currentSceneName == SceneManager.GetActiveScene().name)
        {
            PlayerData player = FindFirstObjectByType<PlayerData>();

            if (player != null)
            {
                player.transform.position = transform.position;
            }
        }
    }

    public void SaveData(GameData data)
    {
        if (!isActive)
            return;

        data.currentCheckpointId = checkpointId;
        data.currentSceneName = SceneManager.GetActiveScene().name;
        data.playerPosX = transform.position.x;
        data.playerPosY = transform.position.y;
        data.playerPosZ = transform.position.z;
    }
}
