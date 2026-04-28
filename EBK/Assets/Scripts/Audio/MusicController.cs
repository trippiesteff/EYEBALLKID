using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance { get; private set; }

    [Header("Music Event")]
    [SerializeField] private EventReference musicEvent;

    [Header("Optional Auto Start")]
    [SerializeField] private bool playOnStart = true;

    private EventInstance musicInstance;
    private bool isInitialized;
    private bool isPlaying;

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
        InitializeMusic();

        if (playOnStart)
            StartMusic();
    }

    private void InitializeMusic()
    {
        if (isInitialized)
            return;

        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        isInitialized = true;
    }

    public void StartMusic()
    {
        if (!isInitialized)
            InitializeMusic();

        PLAYBACK_STATE playbackState;
        musicInstance.getPlaybackState(out playbackState);

        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            musicInstance.start();
            isPlaying = true;
        }
    }

    public void StopMusic()
    {
        if (!isInitialized)
            return;

        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isPlaying = false;
    }

    public void StopMusicImmediate()
    {
        if (!isInitialized)
            return;

        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        isPlaying = false;
    }

    public void PauseMusic(bool paused)
    {
        if (!isInitialized)
            return;

        musicInstance.setPaused(paused);
        isPlaying = !paused;
    }

    public void SetParameter(string parameterName, float value)
    {
        if (!isInitialized)
            return;

        musicInstance.setParameterByName(parameterName, value);
    }

    public void SetParameter(string parameterName, int value)
    {
        if (!isInitialized)
            return;

        musicInstance.setParameterByName(parameterName, value);
    }

    public void SetParameterWithLabel(string parameterName, string label)
    {
        if (!isInitialized)
            return;

        musicInstance.setParameterByNameWithLabel(parameterName, label);
    }

    public void SetMusicState(string label)
    {
        SetParameterWithLabel("MusicState", label);
    }

    public void SetCombatIntensity(float value)
    {
        SetParameter("CombatIntensity", value);
    }

    public void SetBossPhase(int phase)
    {
        SetParameter("BossPhase", phase);
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (!isInitialized)
            return;

        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
