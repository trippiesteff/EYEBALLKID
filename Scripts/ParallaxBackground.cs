using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public static ParallaxBackground instance;

    private void Awake()
    {
        instance = this;
    }

    private Transform theCam;

    public Transform sky, treeline, foregroundObjects, backgroundObjects;

    [Range(0f,1f)]
    public float parallaxSpeed;
    
    public float parallaxSpeedForegroundObjects;
    public float parallaxSpeedBackgroundObjects;

    public float verticalOffset;
    // private float adjustedYOffset;

    
    // Start is called before the first frame update
    void Start()
    {
        theCam = Camera.main.transform;
        // float adjustedYOffset = (((Camera.main.orthographicSize * 2f)/2f) + verticalOffset);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // MoveBackground();
        // sky.position = new Vector3(theCam.position.x, theCam.position.y, sky.position.z);
        // treeline.position = new Vector3(
        //     theCam.position.x * parallaxSpeed,theCam.position.y * parallaxSpeed, sky.position.z);
    }

    public void MoveBackground()
    {
        sky.position = new Vector3(theCam.position.x, theCam.position.y, sky.position.z);
        treeline.position = new Vector3(theCam.position.x * parallaxSpeed,theCam.position.y * parallaxSpeed, sky.position.z);
        foregroundObjects.position = new Vector3(theCam.position.x * parallaxSpeedForegroundObjects,theCam.position.y * parallaxSpeedForegroundObjects - (((Camera.main.orthographicSize * 2f)/2f) + verticalOffset), sky.position.z);
        backgroundObjects.position = new Vector3(theCam.position.x * parallaxSpeedBackgroundObjects,theCam.position.y * parallaxSpeedBackgroundObjects - (((Camera.main.orthographicSize * 2f)/2f) + verticalOffset), sky.position.z);
    }
}
