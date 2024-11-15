using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    public GameObject [] projectilesThatShouldNotRotate;
    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject projectile in projectilesThatShouldNotRotate)
        {
            if (projectile != null) // Ensure the GameObject is not null
            {
                projectile.transform.rotation = initialRotation;
            }
    }
    }
}
