using UnityEngine;
using System.Collections;
using System;

public class CollisionDetection : MonoBehaviour 
{

    public event Action<Collider> TriggerEnter;
    private void FireTriggerEnter(Collider collider)
    {
        if (TriggerEnter != null)
        {
            TriggerEnter(collider);
        }
    }
    public event Action<Collider> TriggerExit;
    private void FireTriggerExit(Collider collider)
    {
        if (TriggerExit != null)
        {
            TriggerExit(collider);
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("collision: " + collider);
        this.FireTriggerEnter(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log("collision: " + collider);
        this.FireTriggerExit(collider);
    }
}
