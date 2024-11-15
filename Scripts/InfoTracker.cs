using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTracker : MonoBehaviour
{
    public static InfoTracker instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            if(PlayerPrefs.HasKey("lives"))
            {
            currentLives = PlayerPrefs.GetInt("lives");
            currentLives = PlayerPrefs.GetInt("fruit");
            }
        }else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int currentLives, currentFruit;

    public void GetInfo()
    {
        if(LifeController.instance != null)
        {
           currentLives = LifeController.instance.currentLives;
        }

        if(CollectiblesManager.instance != null)   
        
        {
            currentFruit = CollectiblesManager.instance.collectibleCount;
        }
    }

    public void SaveInfo()
    {
        PlayerPrefs.SetInt("lives", currentLives);
        PlayerPrefs.SetInt("fruit",currentFruit);
    }
}
