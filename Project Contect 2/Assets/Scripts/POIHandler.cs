using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIHandler : MonoBehaviour
{
    [SerializeField] List<PointOfInterest> pointsOfInterest;
    [SerializeField] GameObject player;

    private PointOfInterest currentPOI = null;

    private void Update()
    {
        CheckForPOI();
    }

    private void CheckForPOI()
    {
        if (currentPOI == null)
        {
            foreach (var _poi in pointsOfInterest)
            {
                float _distance = Vector3.Distance(player.transform.position, _poi.transform.position);
                if (_distance < _poi.FocusDistance)
                {
                    currentPOI = _poi;
                    CinematicCameraHandler.Instance.SetFocus(_poi.transform);
                    break;
                }
            }
        }
        else
        {
            float _distance = Vector3.Distance(player.transform.position, currentPOI.transform.position);
            if (_distance > currentPOI.FocusDistance)
            {
                currentPOI = null;
                CinematicCameraHandler.Instance.StopFocus();
            }
        }
    }
}
