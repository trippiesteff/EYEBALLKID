using UnityEngine;

public class SceneTransitionDestination : MonoBehaviour
{
    [SerializeField] private string destinationId;

    public string DestinationId => destinationId;
}
