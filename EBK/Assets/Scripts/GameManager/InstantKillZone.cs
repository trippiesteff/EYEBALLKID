using UnityEngine;

public class InstantKillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Player player = collision.GetComponent<Player>();
        if (player == null)
            return;

        player.EntityDeath();
    }
}
