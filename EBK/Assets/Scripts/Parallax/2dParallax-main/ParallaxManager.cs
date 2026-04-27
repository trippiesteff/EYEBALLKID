// //Author: Small Hedge Games
// //Date: 13/06/2024

// using UnityEngine;
// using System;

// public class ParallaxManager : MonoBehaviour
// {
//     [SerializeField] private Background[] backgrounds;
//     [SerializeField] private bool isBackground;
//     [SerializeField] private bool changeAnchorsOnStart;
//     [SerializeField] private float heightMultiplier;
//     [SerializeField] private Transform anchor;

//     public Transform gameCamera;
//     public static ParallaxManager instance;
//     private Vector3 anchorPosition;
    

//     private void Awake()
//     {
//         instance = this;
//     }

//     private void Start()
//     {
//         if (!anchor) anchorPosition = gameCamera.position;

//         if(changeAnchorsOnStart)
//             for (int i = 0; i < backgrounds.Length; i++)
//                 backgrounds[i].anchor = backgrounds[i].sprite.position;
//     }

//     private void Update()
//     {
//         for (int i = 0; i < backgrounds.Length; i++)
//         {
//             if (anchor) anchorPosition = anchor.position;
//             float adjustedIntensity = backgrounds[i].intensity;
//             if (isBackground) --adjustedIntensity;
//             backgrounds[i].sprite.position = (Vector2)anchorPosition + backgrounds[i].anchor + new Vector2(-adjustedIntensity, adjustedIntensity * heightMultiplier) * (gameCamera.position - anchorPosition);
//         }
//     }

//     public void ChangeAnchor(int index, Vector2 anchor)
//     {
//         backgrounds[index].anchor = anchor;
//     }

//     public void ResetBackgrounds()
//     {
//         Array.Resize(ref backgrounds, transform.childCount);
//         for (int i = 0; i < backgrounds.Length; i++)
//         {
//             if (!backgrounds[i].sprite) 
//                 backgrounds[i].intensity = GetIntensity(i);

//             backgrounds[i].sprite = transform.GetChild(i);
//             backgrounds[i].name = backgrounds[i].sprite.name;
//         }
//     }

//     public void ResetIntensities()
//     {
//         for (int i = 0; i < backgrounds.Length; ++i)
//             backgrounds[i].intensity = (float)(i + 1) / (backgrounds.Length + 1);
//     }

//     public float GetIntensity(int index)
//     {
//         return backgrounds[index].intensity;
//     }
// }

// [Serializable]
// public struct Background
// {
//     [HideInInspector] public string name;
//     [Range(0, 1)] public float intensity;
//     public Transform sprite;
//     public Vector2 anchor;
// }

// Author: Small Hedge Games (modified)
// Date: 13/06/2024 (updated)

using UnityEngine;
using System;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private Background[] backgrounds;
    [SerializeField] private bool isBackground;
    [SerializeField] private bool changeAnchorsOnStart;
    [SerializeField] private float heightMultiplier = 1f;
    [SerializeField] private Transform anchor;       // optional origin override

    public Transform gameCamera;
    public static ParallaxManager instance;

    // NEW: store where things were when the scene started
    private Vector3[] initialSpritePositions;
    private Vector3 initialCameraPosition;
    private bool initialized = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // If backgrounds array is empty, auto-fill from children
        if (backgrounds == null || backgrounds.Length == 0)
        {
            ResetBackgrounds();
        }

        // Allocate the array for initial positions
        if (initialSpritePositions == null || initialSpritePositions.Length != backgrounds.Length)
        {
            initialSpritePositions = new Vector3[backgrounds.Length];
        }

        // Record where each sprite is placed in the scene
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (!backgrounds[i].sprite)
            {
                backgrounds[i].sprite = transform.GetChild(i);
            }

            // Base position without parallax; anchor is treated as an extra offset
            initialSpritePositions[i] = backgrounds[i].sprite.position - (Vector3)backgrounds[i].anchor;
        }

        // IMPORTANT:
        // We do NOT use the camera position here, because Cinemachine
        // will still move it after Start(). We grab it in LateUpdate instead.
        
        if (changeAnchorsOnStart)
        {
            // Keep this behaviour for compatibility: anchor = current sprite position
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].anchor = (Vector2)backgrounds[i].sprite.position;
            }
        }
    }

    private void FixedUpdate()
    {
        // First frame after everything (including Cinemachine) has moved
        if (!initialized)
        {
            // Where the camera "really starts" for parallax
            initialCameraPosition = anchor ? anchor.position : gameCamera.position;
            initialized = true;
            return; // skip one frame so nothing jumps
        }

        // Camera movement since the start
        Vector3 camDelta = gameCamera.position - initialCameraPosition;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float adjustedIntensity = backgrounds[i].intensity;

            // Preserve original behaviour: backgrounds move differently if flagged
            if (isBackground)
                adjustedIntensity -= 1f; // same as "--adjustedIntensity" but clearer

            // How much this layer should move relative to camera delta
            Vector2 parallaxOffset = new Vector2(
                -adjustedIntensity * camDelta.x,
                adjustedIntensity * heightMultiplier * camDelta.y
            );

            // Final position = original placement + anchor + parallax offset
            Vector2 basePos = (Vector2)initialSpritePositions[i] + backgrounds[i].anchor;
            backgrounds[i].sprite.position = basePos + parallaxOffset;
        }
    }

    public void ChangeAnchor(int index, Vector2 anchor)
    {
        backgrounds[index].anchor = anchor;
    }

    public void ResetBackgrounds()
    {
        Array.Resize(ref backgrounds, transform.childCount);
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (!backgrounds[i].sprite)
                backgrounds[i].intensity = GetIntensity(i);

            backgrounds[i].sprite = transform.GetChild(i);
            backgrounds[i].name = backgrounds[i].sprite.name;
        }
    }

    public void ResetIntensities()
    {
        for (int i = 0; i < backgrounds.Length; ++i)
            backgrounds[i].intensity = (float)(i + 1) / (backgrounds.Length + 1);
    }

    public float GetIntensity(int index)
    {
        return backgrounds[index].intensity;
    }
}

[Serializable]
public struct Background
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float intensity;
    public Transform sprite;
    public Vector2 anchor;
}