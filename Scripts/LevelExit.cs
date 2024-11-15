using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    // public Animator anim;
    private bool isEnding;

    public string nextLevel;

    public float waitToEndLevel = 2f;

    public GameObject blocker;

    public float fadeTime = 1f;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnding == false)
        {
        
            if(other.CompareTag("Player"))
            {
                // anim.SetTrigger("ended");
                Debug.Log("Level Exit");
                isEnding = true;
                AudioManager.Instance.PlayLevelCompleteMusic();
                blocker.SetActive(true);

                StartCoroutine(EndLevelCo());
            }
    }
    }

    IEnumerator EndLevelCo()
    {
         yield return new WaitForSeconds(waitToEndLevel - fadeTime);

        UIController.instance.FadeToBlack();

        yield return new WaitForSeconds(fadeTime);

        InfoTracker.instance.GetInfo();
        InfoTracker.instance.SaveInfo();

        // if (nextLevel != "Victory Screen")
        // {
            PlayerPrefs.SetString("currentLevel", nextLevel);

        // }

        SceneManager.LoadScene(nextLevel);
    }
}
