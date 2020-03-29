using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class POITrigger : MonoBehaviour
{
    [SerializeField] PointOfInterest correspondingPointOfInterest;
    [SerializeField] CinemachineVirtualCamera cinematicCamera;
    [SerializeField] CinemachineVirtualCamera paintingCamera;
    [SerializeField] AudioClip audioClip;
    [SerializeField] float extraPaintDelay;
    [SerializeField] UnityEvent unityEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            float _duration = (audioClip == null ? 3 : audioClip.length) + extraPaintDelay;

            UIManager.Instance.SetActiveLetterbox(true);
            POIHandler.Instance.ActivatePointOfInterest(correspondingPointOfInterest);
            CameraController.Instance.SetPOICamera(cinematicCamera, paintingCamera, _duration);
            AudioManager.Instance.PlayClip(audioClip);

            if(unityEvent != null) { unityEvent.Invoke(); }

            gameObject.SetActive(false);
        }
    }
}
