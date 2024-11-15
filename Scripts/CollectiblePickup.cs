using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePickup : MonoBehaviour
{
    public Animator anim;

    public int amount = 1;

    public GameObject pickupEffect;

    private bool isCollected = false;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
     if (other.CompareTag("Player") && !isCollected)
     {
        if(CollectiblesManager.instance != null)
        {
            CollectiblesManager.instance.GetCollectible(amount);

            anim.SetBool("ShroomsActive", true);

            Instantiate (pickupEffect, transform.position, Quaternion.identity);

            isCollected = true;

            AudioManager.Instance.PlaySFXPitched(9);
        
        }
     }   
    }
}
