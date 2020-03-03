using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinematicCameraHandler : MonoBehaviour
{
    public static CinematicCameraHandler Instance;

    [SerializeField] CinemachineVirtualCamera mainCamera;
    [SerializeField] CinemachineVirtualCamera groupCamera;
    [SerializeField] CinemachineTargetGroup targetGroup;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this) { Destroy(gameObject); }
        }
        else { Instance = this; DontDestroyOnLoad(Instance); }
        SetupCameras();
    }

    public void SetupCameras()
    {
        mainCamera = GameObject.Find("CM_Main").GetComponent< CinemachineVirtualCamera>();
        groupCamera = GameObject.Find("CM_GroupCam").GetComponent<CinemachineVirtualCamera>();
        targetGroup = GameObject.Find("TargetGroup1").GetComponent<CinemachineTargetGroup>();

        groupCamera.m_LookAt = targetGroup.transform;
    }

    public void SetFocus(Transform _focusPoint)
    {
        Debug.Log("WE ARE ACTUALLY FOCUSSING | target = " + _focusPoint.gameObject);
        targetGroup.m_Targets[1].target = _focusPoint;
        mainCamera.m_Priority = 0;
        groupCamera.m_Priority = 1;
    }

    public void StopFocus()
    {
        groupCamera.m_Priority = 0;
        mainCamera.m_Priority = 1;
    }
}
