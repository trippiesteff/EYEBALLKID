using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicPlayer : MonoBehaviour
{
    public int trackToPlay;
    // Start is called before the first frame update
    void Start()
    {   
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelMusic(trackToPlay);
        }
    }
}
