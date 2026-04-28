using System.Collections;
using UnityEngine;

public class SoftPitZone : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float reTriggerDelay = 0.5f;
    [SerializeField] private float postTeleportDelay = 0.05f;

    private float lastTriggerTime = -999f;
    private bool isProcessing;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isProcessing)
            return;

        if (Time.time < lastTriggerTime + reTriggerDelay)
            return;

        if (!collision.CompareTag("Player"))
            return;

        Player player = collision.GetComponent<Player>();
        Entity_Health health = collision.GetComponent<Entity_Health>();

        if (player == null || health == null)
            return;

        StartCoroutine(SoftPitRoutine(player, health));
    }

    private IEnumerator SoftPitRoutine(Player player, Entity_Health health)
    {
        isProcessing = true;
        lastTriggerTime = Time.time;

        float healthBefore = health.GetCurrentHealth();
        health.ReduceHealth(damageAmount);

        if (healthBefore <= damageAmount)
        {
            isProcessing = false;
            yield break;
        }

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeOut();

        SoftRespawnPoint nearestPoint = FindNearestPoint(player.transform.position);
        if (nearestPoint != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            player.transform.position = nearestPoint.transform.position;
        }

        if (postTeleportDelay > 0f)
            yield return new WaitForSecondsRealtime(postTeleportDelay);

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeIn();

        isProcessing = false;
    }

    private SoftRespawnPoint FindNearestPoint(Vector3 fromPosition)
    {
        SoftRespawnPoint[] points = FindObjectsByType<SoftRespawnPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        SoftRespawnPoint nearest = null;
        float bestDistance = Mathf.Infinity;

        foreach (SoftRespawnPoint point in points)
        {
            float distance = Vector3.Distance(fromPosition, point.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                nearest = point;
            }
        }

        return nearest;
    }
}
