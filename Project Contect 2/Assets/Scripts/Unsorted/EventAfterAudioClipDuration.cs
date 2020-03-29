using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventAfterAudioClipDuration : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;
    [SerializeField] UnityEvent unityEvent;

    private void Awake()
    {
        Invoke("InvokeEvent", audioClip.length);
    }

    private void InvokeEvent()
    {
        unityEvent.Invoke();
    }
}
