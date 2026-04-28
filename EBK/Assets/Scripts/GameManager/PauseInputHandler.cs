using UnityEngine;

public class PauseInputHandler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TogglePause();
            }
        }
    }
}
