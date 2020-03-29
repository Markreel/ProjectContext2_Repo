using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class FinalCameraBehaviour : MonoBehaviour
{
    [SerializeField] float targetFielfOfView;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] bool isMuseumCamera;

    [SerializeField] GameObject credits;
    [SerializeField] float creditsFirstStop;
    [SerializeField] float creditsSecondStop;
    [SerializeField] float creditsFinalStop;

    private float currentFieldOfView;
    private Vector3 startPos;

    private CinemachineVirtualCamera vCam;
    private Camera cam;

    private void Awake()
    {
        if (!isMuseumCamera)
        {
            vCam = GetComponent<CinemachineVirtualCamera>();
            currentFieldOfView = vCam.m_Lens.FieldOfView;
        }
        else
        {
            startPos = transform.localPosition;
            cam = GetComponent<Camera>();
            currentFieldOfView = cam.fieldOfView;
        }
    }

    public void ChangeFieldOfView(float _delay)
    {
        StartCoroutine(IEChangeFieldOfView(_delay));
    }

    private IEnumerator IEChangeFieldOfView(float _delay)
    {
        if (_delay != 0) { yield return new WaitForSeconds(_delay); }

        float _lerpTime = 0;

        while (_lerpTime < 1)
        {
            _lerpTime += Time.deltaTime / duration;
            float _timeKey = animationCurve.Evaluate(_lerpTime);

            if (!isMuseumCamera) { vCam.m_Lens.FieldOfView = Mathf.Lerp(currentFieldOfView, targetFielfOfView, _timeKey); }
            else { cam.fieldOfView = Mathf.Lerp(currentFieldOfView, targetFielfOfView, _timeKey); }
            yield return null;
        }


        if (isMuseumCamera)
        {
            AudioManager.Instance.PlayCreditsMusic();
            yield return new WaitForSeconds(1f);

            _lerpTime = 0;

            while (_lerpTime < 1)
            {
                _lerpTime += Time.deltaTime / 3;
                float _timeKey = animationCurve.Evaluate(_lerpTime);

                transform.localPosition = Vector3.Lerp(startPos, startPos + new Vector3(1.6f, 0, 0), _timeKey);
                yield return null;
            }

            yield return new WaitForSeconds(3);

            //Credits

            Vector3 _creditsStart = credits.transform.localPosition;
            Vector3 _creditsFirstStop = new Vector3(_creditsStart.x, creditsFirstStop, _creditsStart.z);
            Vector3 _creditsSecondStop = new Vector3(_creditsStart.x, creditsSecondStop, _creditsStart.z);
            Vector3 _creditsFinalStop = new Vector3(_creditsStart.x, creditsFinalStop, _creditsStart.z);

            //Logo to middle
            _lerpTime = 0;
            while (_lerpTime < 1)
            {
                _lerpTime += Time.deltaTime / 5;
                float _timeKey = animationCurve.Evaluate(_lerpTime);

                credits.transform.localPosition = Vector3.Lerp(_creditsStart, _creditsFirstStop, _timeKey);
                yield return null;
            }

            yield return new WaitForSeconds(3);
            _creditsStart = credits.transform.localPosition;

            //Logo out of screen
            _lerpTime = 0;
            while (_lerpTime < 1)
            {
                _lerpTime += Time.deltaTime / 2.5f;
                float _timeKey = animationCurve.Evaluate(_lerpTime);

                credits.transform.localPosition = Vector3.Lerp(_creditsFirstStop, _creditsSecondStop, _timeKey);
                yield return null;
            }

            _creditsStart = credits.transform.localPosition;

            //Names
            _lerpTime = 0;
            while (_lerpTime < 1)
            {
                _lerpTime += Time.deltaTime / 75; //KIJK HIER ZO NAAR (LAAT LOGO SNEL OMHOOG GAAN EN DAARNA SLOOM TEKST EN VOEG OOK AUDIO IN KLOOF TOE AMIN
                float _timeKey = animationCurve.Evaluate(_lerpTime);

                credits.transform.localPosition = Vector3.Lerp(_creditsSecondStop, _creditsFinalStop, _timeKey);
                yield return null;
            }

            AudioManager.Instance.PlayMusicClip(null);
            UIManager.Instance.FadeOut(3);

            yield return new WaitForSeconds(3);

            Application.Quit();
        }

        yield return null;
    }
}
