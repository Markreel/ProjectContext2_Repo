using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    Image image;
    public bool IsEnabled;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        image = GetComponent<Image>();
        Disable();
    }

    public void Enable(float _duration)
    {
        IsEnabled = true;
        fadeRoutine = StartCoroutine(IEFadeIn(_duration));
    }

    public void Disable()
    {
        if(fadeRoutine != null) { StopCoroutine(fadeRoutine); }

        IsEnabled = false;
        image.color = Color.clear;
    }

    public void DisableWithDelay()
    {
        if (fadeRoutine != null) { StopCoroutine(fadeRoutine); }
        image.color = Color.white;

        IsEnabled = false;
        StartCoroutine(IEDisableWithDelay());
    }

    private IEnumerator IEDisableWithDelay()
    {
        yield return new WaitForSeconds(2f);

        float _lerpTime = 0;
        while (_lerpTime < 1)
        {
            _lerpTime += Time.deltaTime / 1.5f;
            AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            float _lerpKey = _curve.Evaluate(_lerpTime);
            image.color = Color.Lerp(Color.white, Color.clear, _lerpKey);

            yield return null;
        }
    }

    private IEnumerator IEFadeIn(float _duration)
    {
        yield return new WaitForSeconds(1.5f);

        float _lerpTime = 0;
        while (_lerpTime < 1)
        {
            _lerpTime += Time.deltaTime / _duration;
            AnimationCurve _curve = AnimationCurve.EaseInOut(0,0,1,1);

            float _lerpKey = _curve.Evaluate(_lerpTime);
            image.color = Color.Lerp(Color.clear, Color.white, _lerpKey);

            yield return null;
        }

        yield return null;
    }
}
