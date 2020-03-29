using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main Menu Settings: ")]
    [SerializeField] GameObject MainMenuWindow;

    [Header("Fade Settings: ")]
    [SerializeField] Image fadeImage;
    [SerializeField] AnimationCurve fadeCurve;

    [Header("Letterbox Settings: ")]
    [SerializeField] GameObject topLetterboxBar;
    [SerializeField] GameObject BottomLetterboxBar;
    [SerializeField] float letterboxTransitionDuration;
    [SerializeField] AnimationCurve letterboxTransitionCurve;

    [Header("Paint Text Settings: ")]
    [SerializeField] TextMeshProUGUI paintText;
    [SerializeField] float paintTextFlickerDuration;
    [SerializeField] AnimationCurve paintTextFlickerCurve;

    private Coroutine fadeRoutine;
    private Coroutine letterboxTransitionRoutine;
    private Coroutine paintTextFadeRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this) { Destroy(gameObject); }
        }
        else { Instance = this; DontDestroyOnLoad(Instance); }
    }

    public void BehaviourOnStart()
    {
        BlackScreen();
        SetActivePaintText(false);
        FadeIn(3);
        //SetActiveLetterbox(true);
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

    public void FadeToWhite(float _duration)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(IEFade(_duration, true, true));
    }

    private IEnumerator IEFade(float _duration, bool _fadeOut = false, bool _isWhite = false)
    {
        Color _blackOrWhite = _isWhite ? Color.white : Color.black;

        Color _beginColor = fadeImage.color;
        Color _endColor = _fadeOut ? _blackOrWhite : Color.clear;

        float _timeKey = 0;

        while (_timeKey < 1)
        {
            _timeKey += Time.deltaTime / _duration;
            float _lerpKey = fadeCurve.Evaluate(_timeKey);

            fadeImage.color = Color.Lerp(_beginColor, _endColor, _lerpKey);
            yield return null;
        }
    }

    public void SetActiveLetterbox(bool _value)
    {
        if (letterboxTransitionRoutine != null) StopCoroutine(letterboxTransitionRoutine);
        letterboxTransitionRoutine = StartCoroutine(IESetActiveLetterbox(_value));
    }

    private IEnumerator IESetActiveLetterbox(bool _value)
    {
        float _lerpTime = 0;
        float _startScale = topLetterboxBar.transform.localScale.y;

        while (_lerpTime < 1)
        {
            _lerpTime += Time.deltaTime / letterboxTransitionDuration;
            float _lerpKey = letterboxTransitionCurve.Evaluate(_lerpTime);

            float yScale = Mathf.Lerp(_startScale, _value ? 1 : 0, _lerpKey);

            topLetterboxBar.transform.localScale = BottomLetterboxBar.transform.localScale = new Vector3(1, yScale, 1);
            yield return null;
        }

        yield return null;
    }

    public void SetActivePaintText(bool _value)
    {
        if (paintTextFadeRoutine != null) StopCoroutine(paintTextFadeRoutine);
        paintTextFadeRoutine = StartCoroutine(IESetActivePaintText(_value));
    }

    private IEnumerator IESetActivePaintText(bool _value)
    {
        if(_value == false) { paintText.alpha = 0; }
        else
        {
            paintText.alpha = 0.25f;
            float _lerpTime = 0;

            while (_lerpTime < 1)
            {
                _lerpTime += Time.deltaTime / paintTextFlickerDuration;
                float _lerpKey = paintTextFlickerCurve.Evaluate(_lerpTime);

                float _newAlphaValue = Mathf.Lerp(0.25f, 1, _lerpKey);
                paintText.alpha = _newAlphaValue;

                yield return null;
            }

            SetActivePaintText(true);
        }
        yield return null;
    }

    public void DisableMainMenu()
    {
        MainMenuWindow.SetActive(false);
    }

}
