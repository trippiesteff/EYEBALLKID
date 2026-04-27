using UnityEngine;

public class SimpleParallax : MonoBehaviour
{
    [Tooltip("Leave empty to use main camera")]
    public Transform cam;

    [Tooltip("0 = no movement, 1 = same as camera, <1 = slower background")]
    public float parallaxMultiplier = 0.5f;

    private Vector3 lastCamPos;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        lastCamPos = cam.position;
    }

    private void LateUpdate()
    {
        // How much did the camera move since last frame?
        Vector3 camDelta = cam.position - lastCamPos;
        lastCamPos = cam.position;

        // Move this layer by a fraction of that movement
        transform.position += new Vector3(
            camDelta.x * parallaxMultiplier,
            camDelta.y * parallaxMultiplier,
            0f
        );
    }
}