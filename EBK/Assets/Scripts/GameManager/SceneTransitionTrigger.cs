using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetDestinationId;
    [SerializeField] private bool requirePlayerTag = true;

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered)
            return;

        if (requirePlayerTag && !collision.CompareTag("Player"))
            return;

        isTriggered = true;

        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadScene(targetSceneName, targetDestinationId);
        }
    }
}
