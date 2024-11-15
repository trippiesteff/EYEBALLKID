using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Animator anim;

    [HideInInspector]
    public bool isDefeated;

    public float jumpForce = 20;


    public float waitToDestroy;
    // Start is called before the first frame update
    void Start()
    {
        if(anim == null)
        {
        anim = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
       if (isDefeated == true)
       {
        waitToDestroy -= Time.deltaTime;
        if (waitToDestroy <= 0)
        {
            Destroy(gameObject);
            AudioManager.Instance.PlaySFX(5);
        }
       } 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(isDefeated == false)
            {
                PlayerHealthController.instance.DamagePlayer();
            }
        }

    
    }
    private void OnTriggerEnter2D (Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            // Destroy(gameObject);

            FindFirstObjectByType<NewPlayerController>().Jump();

            anim.SetTrigger("defeated");
            isDefeated = true;

            AudioManager.Instance.PlaySFX(6);

        }
    }
}
