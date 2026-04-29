using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour, IDataPersistence
{
    public static AudioManager Instance { get; private set; }

    [Header("FMOD Events")]
    [SerializeField] private FMODEvents fmodEvents;

    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float ambienceVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float uiVolume = 1f;

    private VCA masterVCA;
    private VCA musicVCA;
    private VCA ambienceVCA;
    private VCA sfxVCA;
    private VCA uiVCA;

    private bool vcasInitialized = false;
    private bool hasLoadedSaveData = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeVCAs();
        ApplyAllVolumes();
        hasLoadedSaveData = true;
    }

    private void InitializeVCAs()
    {
        if (vcasInitialized)
            return;

        masterVCA = RuntimeManager.GetVCA("vca:/Master");
        musicVCA = RuntimeManager.GetVCA("vca:/Music");
        ambienceVCA = RuntimeManager.GetVCA("vca:/Ambience");
        sfxVCA = RuntimeManager.GetVCA("vca:/SFX");
        uiVCA = RuntimeManager.GetVCA("vca:/UI");

        vcasInitialized = true;
    }

    public void PlayPlayerJump(Vector3 position)
    {
        RuntimeManager.PlayOneShot(fmodEvents.playerJump, position);
    }

    public void PlayPlayerDash(Vector3 position)
    {
        RuntimeManager.PlayOneShot(fmodEvents.playerDash, position);
    }

    public void PlayUIClick(Vector3 position)
    {
        RuntimeManager.PlayOneShot(fmodEvents.uiClick, position);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (vcasInitialized && masterVCA.isValid()) masterVCA.setVolume(masterVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (vcasInitialized && musicVCA.isValid()) musicVCA.setVolume(musicVolume);
    }

    public void SetAmbienceVolume(float volume)
    {
        ambienceVolume = Mathf.Clamp01(volume);
        if (vcasInitialized && ambienceVCA.isValid()) ambienceVCA.setVolume(ambienceVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (vcasInitialized && sfxVCA.isValid()) sfxVCA.setVolume(sfxVolume);
    }

    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        if (vcasInitialized && uiVCA.isValid()) uiVCA.setVolume(uiVolume);
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetAmbienceVolume() => ambienceVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetUIVolume() => uiVolume;

    public void ApplyAllVolumes()
    {
        if (!vcasInitialized)
            return;

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetAmbienceVolume(ambienceVolume);
        SetSFXVolume(sfxVolume);
        SetUIVolume(uiVolume);
    }

    public void LoadData(GameData data)
    {
        if (data == null)
            return;

        masterVolume = data.masterVolume;
        musicVolume = data.musicVolume;
        ambienceVolume = data.ambienceVolume;
        sfxVolume = data.sfxVolume;
        uiVolume = data.uiVolume;

        if (vcasInitialized)
        {
            ApplyAllVolumes();
        }
    }

    public void SaveData(GameData data)
    {
        data.masterVolume = masterVolume;
        data.musicVolume = musicVolume;
        data.ambienceVolume = ambienceVolume;
        data.sfxVolume = sfxVolume;
        data.uiVolume = uiVolume;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        if (!hasLoadedSaveData)
            return;

        ApplyAllVolumes();
    }
}