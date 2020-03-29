using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class DollyTrackAnimator : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera dollyCamera;

    [Header("Settings: ")]
    [SerializeField] bool animateOnAwake = false;
    [SerializeField] float animationDelay;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve animationCurve;

    [Header("PostAnimationEvent")]
    [SerializeField] float eventDelay;
    [SerializeField] UnityEvent postAnimationEvent;

    private Coroutine animateCameraOverTrackRoutine;

    private void Awake()
    {
        if (animateOnAwake) { AnimateCameraOverTrack(); }
    }

    public void AnimateCameraOverTrack()
    {
        if (animateCameraOverTrackRoutine != null) StopCoroutine(animateCameraOverTrackRoutine);
        animateCameraOverTrackRoutine = StartCoroutine(IEAnimateCameraOverTrack());
    }

    public void AnimateCameraOverTrack(float _duration)
    {
        duration = _duration;

        if (animateCameraOverTrackRoutine != null) StopCoroutine(animateCameraOverTrackRoutine);
        animateCameraOverTrackRoutine = StartCoroutine(IEAnimateCameraOverTrack());
    }

    private IEnumerator IEAnimateCameraOverTrack()
    {
        yield return new WaitForSeconds(animationDelay);

        float _lerpTime = 0;
        while (_lerpTime < 1)
        {
            _lerpTime += Time.deltaTime / duration;

            float _lerpKey = animationCurve.Evaluate(_lerpTime);
            dollyCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = _lerpKey;
            yield return null;
        }

        yield return new WaitForSeconds(eventDelay);

        postAnimationEvent.Invoke();

        yield return null;
    }
}
