using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Fade Settings: ")]
    [SerializeField] Image fadeImage;
    [SerializeField] AnimationCurve fadeCurve;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this) { Destroy(gameObject); }
        }
        else { Instance = this; DontDestroyOnLoad(Instance); }
    }

    public void BlackScreen()
    {
        fadeImage.color = Color.black;
    }

    public void FadeIn(float _duration)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(IEFade(_duration));
    }

    public void FadeOut(float _duration)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(IEFade(_duration, true));
    }

    private IEnumerator IEFade(float _duration, bool _fadeOut = false)
    {
        Color _beginColor = fadeImage.color;
        Color _endColor = _fadeOut ? Color.black : Color.clear;

        float _timeKey = 0;

        while (_timeKey < 1)
        {
            _timeKey += Time.deltaTime / _duration;
            float _lerpKey = fadeCurve.Evaluate(_timeKey);

            fadeImage.color = Color.Lerp(_beginColor, _endColor, _lerpKey);
            yield return null;
        }
    }
}
