using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    // Start is called before the first frame update
     void Awake()
    {
        if(Instance == null)
        {
            // Instance = this;
            // DontDestroyOnLoad(gameObject);
            SetupAudioManager();
        }else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetupAudioManager()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public AudioSource menuMusic, bossMusic, levelCompleteMusic;

    public AudioSource[] levelTracks;

    public AudioSource[] allSFX;

    void StopMusic()
    {
        menuMusic.Stop();
        bossMusic.Stop();
        levelCompleteMusic.Stop();

        foreach (AudioSource track in levelTracks)
        {
            track.Stop();
        }
    }

public void PlayMenuMusic()
{
    StopMusic();
    menuMusic.Play();
}

public void PlayBossMusic()
{
    StopMusic();
    bossMusic.Play();
}

public void PlayLevelCompleteMusic()
{
    StopMusic();
    levelCompleteMusic.Play();
}

public void PlayLevelMusic(int trackToPlay)
{
    StopMusic();
    levelTracks[trackToPlay].Play();
}

public void PlaySFX(int sfxToPlay)
{
    Debug.Log("Playing SFX: " + sfxToPlay);
    allSFX[sfxToPlay].Stop();
    allSFX[sfxToPlay].Play();

}

public void PlaySFXPitched(int sfxToPlay)
{
    Debug.Log("Playing Pitched SFX: " + sfxToPlay);
    allSFX[sfxToPlay].Stop();
    allSFX[sfxToPlay].pitch = Random.Range(.75f,1.25f);
    allSFX[sfxToPlay].Play();
}
}
