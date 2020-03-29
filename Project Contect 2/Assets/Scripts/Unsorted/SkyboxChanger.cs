using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    [Header("Color: ")]
    [SerializeField] Color oldColorBackup;
    [SerializeField] Color finalSkyColor;
    private Color startSkyColor;

    [Header("Exposure: ")]
    [SerializeField] float oldExposureBackup;
    [SerializeField] float finalExposureValue;

    [Header("Rotation: ")]
    [SerializeField] float oldRotationBackup = 245;
    [SerializeField] float finalRotation = 80;

    [Header("Settings: ")]
    [SerializeField] float lerpDuration;
    [SerializeField] AnimationCurve lerpCurve;

    private Coroutine lerpSkyColorRoutine;

    public void SetNormalSkybox()
    {
        RenderSettings.skybox.SetColor("_Tint", oldColorBackup);
        RenderSettings.skybox.SetFloat("_Exposure", oldExposureBackup);
        RenderSettings.skybox.SetFloat("_Rotation", oldRotationBackup);
    }

    public void ChangeRotation()
    {
        RenderSettings.skybox.SetFloat("_Rotation", finalRotation);
    }

    public void LerpSkyColor()
    {
        if (lerpSkyColorRoutine != null) { StopCoroutine(lerpSkyColorRoutine); }
        lerpSkyColorRoutine = StartCoroutine(IELerpSkyColor());
    }

    private IEnumerator IELerpSkyColor()
    {
        startSkyColor = oldColorBackup; //RenderSettings.skybox.GetColor("_Tint");

        float _lerpTime = 0;
        while (_lerpTime < 1)
        {
            _lerpTime += Time.deltaTime / lerpDuration;
            float _lerpKey = lerpCurve.Evaluate(_lerpTime);

            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(startSkyColor, finalSkyColor, _lerpKey));
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(oldExposureBackup, finalExposureValue, _lerpKey));

            yield return null;
        }

        yield return null;
    }
}
