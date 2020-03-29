using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeEvent : MonoBehaviour
{
    [SerializeField] bool invokeOnStart;
    [SerializeField] UnityEvent unityEvent;

    private void Start()
    {
        if (invokeOnStart) InvokeAfterDelay();
    }

    public void InvokeWithDelay(float _delay)
    {
        Invoke("InvokeAfterDelay", _delay);
    }

    private void InvokeAfterDelay()
    {
        unityEvent.Invoke();
    }
}
