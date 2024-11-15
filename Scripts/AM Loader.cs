using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AMLoader : MonoBehaviour
{
    public AudioManager theAM;

    private void Awake()
    {
        if (AudioManager.Instance == null)
        {
            Instantiate(theAM).SetupAudioManager();
        }
    }
}
