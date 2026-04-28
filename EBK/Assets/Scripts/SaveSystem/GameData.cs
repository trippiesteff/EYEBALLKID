using System;
using System.Collections.Generic;

[Serializable]
public class SpecialItemSaveData
{
    public string itemId;
    public int amount;

    public SpecialItemSaveData(string itemId, int amount)
    {
        this.itemId = itemId;
        this.amount = amount;
    }
}

[Serializable]
public class GameData
{
    public long lastUpdated;

    public string currentSceneName;
    public string currentCheckpointId;
    public string pendingDestinationId;

    public float playerHealth;
    public int unlockedHeartContainers;

    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public List<string> ownedWeaponIds;
    public string equippedWeaponId;

    public List<string> ownedToolIds;
    public string equippedToolId;

    public List<SpecialItemSaveData> specialItems;

    public List<string> unlockedAbilityIds;

    public float masterVolume;
    public float musicVolume;
    public float ambienceVolume;
    public float sfxVolume;
    public float uiVolume;

    public GameData()
    {
        currentSceneName = "";
        currentCheckpointId = "";
        pendingDestinationId = "";

        playerHealth = 3f;
        unlockedHeartContainers = 3;

        playerPosX = 0f;
        playerPosY = 0f;
        playerPosZ = 0f;

        ownedWeaponIds = new List<string>();
        equippedWeaponId = "";

        ownedToolIds = new List<string>();
        equippedToolId = "";

        specialItems = new List<SpecialItemSaveData>();
        unlockedAbilityIds = new List<string>();

        masterVolume = 1f;
        musicVolume = 1f;
        ambienceVolume = 1f;
        sfxVolume = 1f;
        uiVolume = 1f;

        lastUpdated = DateTime.Now.ToBinary();
    }
}
