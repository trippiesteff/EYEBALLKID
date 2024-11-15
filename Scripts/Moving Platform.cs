using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public Transform thePlatform;

    public Transform[] movePoints;

    private int currentPoint;

    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        thePlatform.position = UnityEngine.Vector3.MoveTowards(

            thePlatform.position,
            movePoints[currentPoint].position,
            moveSpeed * Time.deltaTime);
        if (thePlatform.position == movePoints[currentPoint].position)
        {
            currentPoint++;

            if(currentPoint>= movePoints.Length)
            {
                currentPoint = 0;
            }    
        }
    }
}
