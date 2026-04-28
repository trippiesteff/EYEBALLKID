using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour, IDataPersistence
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Entity_Health entityHealth;
    [SerializeField] private Entity_Stats entityStats;

    private void Awake()
    {
        if (playerTransform == null)
            playerTransform = transform;

        if (entityHealth == null)
            entityHealth = GetComponent<Entity_Health>();

        if (entityStats == null)
            entityStats = GetComponent<Entity_Stats>();
    }

    public void LoadData(GameData data)
    {
        if (data == null)
            return;

        if (playerTransform == null)
            playerTransform = transform;

        if (entityHealth == null)
            entityHealth = GetComponent<Entity_Health>();

        if (entityStats == null)
            entityStats = GetComponent<Entity_Stats>();

        float maxHealth = entityStats != null ? entityStats.GetMaxHealth() : 3f;

        if (entityHealth != null)
        {
            entityHealth.SetCurrentHealth(maxHealth);
        }

        if (!string.IsNullOrEmpty(data.currentCheckpointId))
            return;

        if (!string.IsNullOrEmpty(data.currentSceneName) &&
            data.currentSceneName == SceneManager.GetActiveScene().name)
        {
            playerTransform.position = new Vector3(
                data.playerPosX,
                data.playerPosY,
                data.playerPosZ
            );
        }
    }

    public void SaveData(GameData data)
    {
        if (playerTransform == null)
            playerTransform = transform;

        if (entityHealth == null)
            entityHealth = GetComponent<Entity_Health>();

        if (entityStats == null)
            entityStats = GetComponent<Entity_Stats>();

        data.currentSceneName = SceneManager.GetActiveScene().name;

        if (entityHealth != null)
        {
            data.playerHealth = entityHealth.GetCurrentHealth();
            data.unlockedHeartContainers = Mathf.RoundToInt(entityHealth.GetMaxHealth());
        }
        else if (entityStats != null)
        {
            data.playerHealth = entityStats.GetMaxHealth();
            data.unlockedHeartContainers = Mathf.RoundToInt(entityStats.GetMaxHealth());
        }
        else
        {
            data.playerHealth = 3f;
            data.unlockedHeartContainers = 3;
        }

        data.playerPosX = playerTransform.position.x;
        data.playerPosY = playerTransform.position.y;
        data.playerPosZ = playerTransform.position.z;
    }

    public void SetCurrentHealth(float health)
    {
        if (entityHealth == null)
            entityHealth = GetComponent<Entity_Health>();

        if (entityHealth != null)
            entityHealth.SetCurrentHealth(health);
    }

    public float GetCurrentHealth()
    {
        if (entityHealth == null)
            entityHealth = GetComponent<Entity_Health>();

        return entityHealth != null ? entityHealth.GetCurrentHealth() : 3f;
    }

    public int GetUnlockedHeartContainers()
    {
        if (entityStats == null)
            entityStats = GetComponent<Entity_Stats>();

        return entityStats != null ? Mathf.RoundToInt(entityStats.GetMaxHealth()) : 3;
    }
}
