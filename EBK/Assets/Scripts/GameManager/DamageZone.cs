using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float hitCooldown = 0.5f;
    [SerializeField] private Transform knockbackSource;

    private float lastHitTime = -999f;

    private void Reset()
    {
        knockbackSource = transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time >= lastHitTime + hitCooldown)
            TryDamage(collision);
    }

    private void TryDamage(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (Time.time < lastHitTime + hitCooldown)
            return;

        Entity_Health health = collision.GetComponent<Entity_Health>();
        if (health == null)
            return;

        Transform source = knockbackSource != null ? knockbackSource : transform;

        health.TakeDamage(damageAmount, 0f, ElementType.None, source);
        lastHitTime = Time.time;
    }
}
