using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent unityEvent;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player" && !activated)
        {
            InvokeAction();
            activated = true;
        }
    }

    private void InvokeAction()
    {
        unityEvent.Invoke();
    }

}
