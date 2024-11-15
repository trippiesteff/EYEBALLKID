using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

public class BossBattleController : MonoBehaviour
{
    private bool bossActive;
    public GameObject blockers;

    public Transform camPoint;
    private CameraController camController;

    public float cameraMoveSpeed;

    public Transform theBoss;
    public float bossGrowSpeed = 2f;

    public Transform projectileLauncher;
    public float launcherGrowSpeed = 2f;

    public float launcherRotateSpeed = 90f;
    private float launcherRotation;

    public GameObject projectileToFire;

    public Transform[] projectilePoints;

    public float waitToStartShooting, timeBetweenShots;

    private float shootStartCounter, shotCounter;

    private int currentShot;

    public Animator bossAnim;

    private bool isWeak;

    public Transform[] bossMovePoints;
    private int currentMovePoint;

    public float bossMoveSpeed;

    private int currentPhase;

    public GameObject deathEffect;

    public float jumpForce = 20;


    // Start is called before the first frame update
    void Start()
    {
        camController = FindFirstObjectByType<CameraController>();

        shootStartCounter = waitToStartShooting;

        blockers.transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        if(bossActive == true)
        {
            camController.transform.position = UnityEngine.Vector3.MoveTowards(camController.transform.position,
            camPoint.position,
            cameraMoveSpeed * Time.deltaTime);

            if (theBoss.localScale != UnityEngine.Vector3.one)
            {
                theBoss.localScale = UnityEngine.Vector3.MoveTowards(
                    theBoss.localScale,
                    UnityEngine.Vector3.one,
                    bossGrowSpeed * Time.deltaTime);

            }
            if (projectileLauncher.localScale != UnityEngine.Vector3.one)
            {
                projectileLauncher.localScale = UnityEngine.Vector3.MoveTowards(
                    projectileLauncher.localScale,
                    UnityEngine.Vector3.one,
                    launcherGrowSpeed * Time.deltaTime);

            }

            launcherRotation += launcherRotateSpeed * Time.deltaTime;

            if(launcherRotation >360f)
            {
                launcherRotation -= 360f;
            }
            
            projectileLauncher.transform.rotation = UnityEngine.Quaternion.Euler(0f,0f, launcherRotation);

            // start shooting

            if(shootStartCounter > 0f)
            {
                shootStartCounter -= Time.deltaTime;
                if(shootStartCounter <= 0f)
                {
                    shotCounter = timeBetweenShots;
                    FireShot();
                    
                }   
            }

            if(shotCounter > 0f)
            {
                shotCounter -= Time.deltaTime;
                if (shotCounter <= 0f)
                {
                    shotCounter = timeBetweenShots;

                    FireShot();
                }
            }

            if(isWeak == false)
            {
                theBoss.transform.position = UnityEngine.Vector3.MoveTowards(
                theBoss.transform.position,
                bossMovePoints[currentMovePoint].position,
                bossMoveSpeed * Time.deltaTime);

                if(theBoss.transform.position == bossMovePoints[currentMovePoint].position)
                {
                    currentMovePoint++;
                    if(currentMovePoint >= bossMovePoints.Length)
                    {
                        currentMovePoint = 0;
                    }  
                }
            }
        
        }

    }

    public void ActivateBattle()
    {
        bossActive = true;

        blockers.SetActive(true);

        camController.enabled = false;

        //audiomanager.instance.PlayBossMusic;
    }

    void FireShot ()
    {
        // Debug.Log("Fired shot at" + Time.time);

        Instantiate(projectileToFire, projectilePoints[currentShot].position, projectilePoints[currentShot].rotation);

        projectilePoints[currentShot].gameObject.SetActive(false);

        currentShot ++;

        if(currentShot>= projectilePoints.Length)
        {
            shotCounter = 0f;
            MakeWeak();
        }

         AudioManager.Instance.PlaySFX(2);
    }

    void MakeWeak()
    {
        bossAnim.SetTrigger("isWeak");
        isWeak = true;

    }

    private void OnCollisionEnter2D(Collision2D other) {
       if(other.gameObject.CompareTag("Player"))
       {
        // Debug.Log("Player Hit");

        if(isWeak == false)
        {
            PlayerHealthController.instance.DamagePlayer();
        
        } else
        {
            if(other.transform.position.y> theBoss.position.y)
            bossAnim.SetTrigger("hit");

            FindFirstObjectByType<NewPlayerController>().Jump();

            MoveToNextPhase();
        }
       } 
    }

    void MoveToNextPhase()
    {
        currentPhase++;

        if (currentPhase < 3)
        {
            isWeak = false;
            waitToStartShooting *= .5f;
            timeBetweenShots *= .75f;
            bossMoveSpeed *= 1.5f;

            shootStartCounter = waitToStartShooting;

            projectileLauncher.localScale = UnityEngine.Vector3.zero;

            foreach(Transform point in projectilePoints)
            {
                point.gameObject.SetActive(true);
            }

            currentShot = 0;

             AudioManager.Instance.PlaySFX(1);



        }else{
            //end battle

            gameObject.SetActive(false);
            blockers.SetActive(false);

            camController.enabled = true;

            Instantiate(deathEffect, theBoss.position, UnityEngine.Quaternion.identity);

            AudioManager.Instance.PlaySFX(0);

             AudioManager.Instance.PlayLevelMusic(FindFirstObjectByType<LevelMusicPlayer>().trackToPlay);
        }

    }

}
