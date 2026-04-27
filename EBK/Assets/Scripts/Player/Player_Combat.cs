using System.Runtime.CompilerServices;
using UnityEngine;

public class Player_Combat : Entity_Combat
{

    [Header("Counter attack details")]
    [SerializeField] private float counterRecovery = .1f;

    public bool CounterAttackPerformed()
    {

        bool hasCounteredSomebody = false;

        foreach(var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if(counterable == null)
            continue;

           if(counterable.CanBeCountered)
           {
           counterable.HandleCounter();
           hasCounteredSomebody = true;
           }
        }
        return hasCounteredSomebody;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;
   
}
