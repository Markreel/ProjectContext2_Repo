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
        Instance = Instance ?? this;
        SetupCameras();
    }

    private void SetupCameras()
    {
        groupCamera.m_LookAt = targetGroup.transform;

    }

    public void SetFocus(Transform _focusPoint)
    {
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
