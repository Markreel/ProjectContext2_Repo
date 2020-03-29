using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] bool playOnAwake = true;
    [SerializeField] bool doLoop = true;

    [SerializeField] float duration;
    [SerializeField] AnimationCurve curve;

    [SerializeField] float hoverRange;

    private Vector3 startPos;
    private Coroutine hoverRoutine;

    private void Awake()
    {
        startPos = transform.position;
        if (playOnAwake) { StartHover(); }
    }

    public void StartHover()
    {
        if (hoverRoutine != null) StopCoroutine(hoverRoutine);
        hoverRoutine = StartCoroutine(IEHover());
    }

    private IEnumerator IEHover()
    {
        float _lerpTime = 0;

        while (_lerpTime < 1)
        {
            _lerpTime += Time.deltaTime / duration;

            float _lerpKey = curve.Evaluate(_lerpTime);
            transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * hoverRange, _lerpKey);
            yield return null;
        }

        if (doLoop) { StartHover(); }

        yield return null;
    }
}
