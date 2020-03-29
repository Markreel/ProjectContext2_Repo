using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] CinemachineVirtualCamera defaultPlayerCamera;

    [Header("IntroCinematicOfDome")]
    [SerializeField] CinemachineVirtualCamera dollyTrackCameraA;

    [Header("On the road to the bridge")]
    [SerializeField] CinemachineVirtualCamera dollyTrackCameraB;

    [Header("Final sprint towards the end")]
    [SerializeField] CinemachineVirtualCamera dollyTrackCameraC;

    [Header("Final Close Up Camera")]
    [SerializeField] CinemachineVirtualCamera finalCloseUpCamera;
    [SerializeField] Camera museumCamera;

    private CinemachineVirtualCamera currentCinematicPOICamera;
    private CinemachineVirtualCamera currentPaintingPOICamera;
    private List<CinemachineVirtualCamera> allCameras = new List<CinemachineVirtualCamera>();

    private void Awake()
    {
        Instance = Instance ?? this;
        StoreAllCameras();
    }

    /// <summary>
    /// Stores all cameras to the AllCameras list so that we can access them later
    /// </summary>
    private void StoreAllCameras()
    {
        allCameras = GetComponentsInChildren<CinemachineVirtualCamera>().ToList();
    }

    private void SetActiveAllCameras(bool _value)
    {
        foreach (var _camera in allCameras)
        {
            if (_camera != null) { _camera.gameObject.SetActive(_value); }
        }
    }

    private void ActivatePaintingCamera(float _delay)
    {
        StartCoroutine(IEActivatePaintingCamera(_delay));
    }

    private IEnumerator IEActivatePaintingCamera(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        UIManager.Instance.FadeOut(3);
        yield return new WaitForSeconds(3);

        SetActiveAllCameras(false);
        currentPaintingPOICamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);

        UIManager.Instance.FadeIn(3);
        yield return new WaitForSeconds(3);

        ArtHandler.Instance.GenerateArtPiece();

        yield return null;
    }

    public void BehaviourOnStart(float _animationDuration)
    {
        SetActiveAllCameras(false);

        dollyTrackCameraA.gameObject.SetActive(true);
        dollyTrackCameraA.GetComponent<DollyTrackAnimator>().AnimateCameraOverTrack(_animationDuration);
    }

    public void ActivateDefaultPlayerCamera()
    {
        SetActiveAllCameras(false);
        defaultPlayerCamera.gameObject.SetActive(true);
    }

    /// <summary>
    /// Changes the camera to the cinematic camera of a Point of Interest and storing the painting camera for later
    /// </summary>
    /// <param name="_cinematicCamera"></param>
    /// <param name="_paintingCamera"></param>
    public void SetPOICamera(CinemachineVirtualCamera _cinematicCamera, CinemachineVirtualCamera _paintingCamera, float _duration)
    {
        currentCinematicPOICamera = _cinematicCamera;
        currentPaintingPOICamera = _paintingCamera;

        SetActiveAllCameras(false);
        currentCinematicPOICamera.gameObject.SetActive(true);

        Invoke("EnableOptionToPaint", _duration);

        //ActivatePaintingCamera(_duration);
    }

    public void StartMuseumSwitch()
    {        
        finalCloseUpCamera.gameObject.SetActive(true);
    }

    public void EnableMuseumCamera(float _delay)
    {
        StartCoroutine(IEEnableMuseumCamera(_delay));
    }

    private IEnumerator IEEnableMuseumCamera(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        museumCamera.gameObject.SetActive(true);
        Camera.main.gameObject.SetActive(false);

        yield return null;
    }

    #region Gadverdamme niet kijken naar deze code anders ga je kotsen (neem een emmer mee)
    private bool canPaint; //Mark het is duidelijk heel lelijk dat je dit hier doet maar vergeef het jezelf want je hebt niet zo veel tijd meer om alles netjes te doen en je bent ook gewoon een mens dus je kan met een gerust hart gaan slapen vanavond, zolang je de deadline maar haalt amen.
    private void EnableOptionToPaint()
    {
        canPaint = true;
        UIManager.Instance.SetActivePaintText(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPaint)
        {
            canPaint = false;
            ActivatePaintingCamera(0);
            UIManager.Instance.SetActivePaintText(false);
        }

    }
    #endregion
}
