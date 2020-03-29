using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class POIHandler : MonoBehaviour
{
    public static POIHandler Instance;
    public List<PointOfInterestData> PointsOfInterestData = new List<PointOfInterestData>();
    public List<Texture2D> Paintings = new List<Texture2D>();

    [SerializeField] List<PointOfInterest> pointsOfInterest;
    [SerializeField] GameObject player;
    [SerializeField] ArtHandler artHandler;
    [SerializeField] UnityEvent onFinalPaintingEnded;

    [Header("Cheap fix UI stuff: ")]
    [SerializeField] GameObject explainationText;
    [SerializeField] TextMeshProUGUI amountText;
    [HideInInspector] public int InspirationAmount;

    public PointOfInterest CurrentPOI = null;
    private List<PointOfInterest> visitedPointsOfInterest = new List<PointOfInterest>();

    private bool alreadyWokeCheapFix = false;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this && !alreadyWokeCheapFix)
            {
                Paintings = Instance.Paintings;
                Destroy(Instance.gameObject);
                Instance = this;
                //DontDestroyOnLoad(Instance);
            }
        }
        else { Instance = this; DontDestroyOnLoad(Instance); }

        //alreadyWokeCheapFix = true;
        //CinematicCameraHandler.Instance.gameObject.SetActive(true);
        //CinematicCameraHandler.Instance.SetupCameras();

        //explainationText.SetActive(false);
    }

    private void Update()
    {
        //if (player != null) CheckForPOI();
        //else { player = GameObject.Find("Player"); }
    }

    public void ActivatePointOfInterest(PointOfInterest _poi)
    {
        player.GetComponent<Hovercraft_V4>().DeactivatePlayer();

        if(!_poi.Visited)
        {
            _poi.Visited = true;
            CurrentPOI = _poi;

            _poi.ScreenShotHandler.TakePicture(true);

            PointOfInterestData _data = new PointOfInterestData(); //Save data for later use
            _data.ArtType = _poi.ArtType;
            _data.ColorPalette = _poi.ColorPalette;
            _data.Picture = (Texture2D)_poi.ScreenShotHandler.CurrentPicture;
            _data.Priority = _poi.Priority;
            _data.Shapes = _poi.Shapes;

            PointsOfInterestData.Add(_data);

            artHandler.UpdatePOIData(_data, PointsOfInterestData);
        }
    }

    public bool DeactivatePointOfInterest()
    {
        if (CurrentPOI.IsFinalPOI) { onFinalPaintingEnded.Invoke(); } //Zoom op kunst, switch naar museum scene, zoom uit en reveal atelier, 
        else
        {
            CurrentPOI.gameObject.SetActive(false);
            player.GetComponent<Hovercraft_V4>().ActivatePlayer();
        }

        return CurrentPOI.IsFinalPOI;
    }

    private void CheckForPOI()
    {
        if (CurrentPOI == null)
        {
            foreach (var _poi in pointsOfInterest)
            {
                float _distance = Vector3.Distance(player.transform.position, _poi.transform.position);
                if (_distance < _poi.FocusDistance && !visitedPointsOfInterest.Contains(_poi) && InspirationAmount != 3)
                {
                    CurrentPOI = _poi;
                    CinematicCameraHandler.Instance.SetFocus(_poi.transform);
                    //explainationText.SetActive(true);
                    Debug.Log("FOCUSSING");

                    //if (visitedPointsOfInterest.Count == 0) { _poi.screenShotHandler.TakePicture(true); } //Take pic with skybox (this will be the background)
                    //else { _poi.screenShotHandler.TakePicture(); } //Take pic of object only (to draw on top of the background picture)

                    //if (!visitedPointsOfInterest.Contains(_poi))
                    //{
                    //    visitedPointsOfInterest.Add(_poi); //Add to list to keep inventory of which POI is visited already

                    //    PointOfInterestData _data = new PointOfInterestData(); //Save data for later use
                    //    _data.ArtType = _poi.ArtType;
                    //    _data.ColorPalette = _poi.ColorPalette;
                    //    _data.Picture = (Texture2D)_poi.screenShotHandler.CurrentPicture;
                    //    _data.Priority = _poi.Priority;
                    //    _data.Shapes = _poi.Shapes;

                    //    PointsOfInterestData.Add(_data);
                    //} 

                    break;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && !visitedPointsOfInterest.Contains(CurrentPOI) && InspirationAmount < 3)
            {
                if (visitedPointsOfInterest.Count == 0) { CurrentPOI.ScreenShotHandler.TakePicture(true); } //Take pic with skybox (this will be the background)
                else { CurrentPOI.ScreenShotHandler.TakePicture(); }

                visitedPointsOfInterest.Add(CurrentPOI); //Add to list to keep inventory of which POI is visited already

                PointOfInterestData _data = new PointOfInterestData(); //Save data for later use
                _data.ArtType = CurrentPOI.ArtType;
                _data.ColorPalette = CurrentPOI.ColorPalette;
                _data.Picture = (Texture2D)CurrentPOI.ScreenShotHandler.CurrentPicture;
                _data.Priority = CurrentPOI.Priority;
                _data.Shapes = CurrentPOI.Shapes;

                PointsOfInterestData.Add(_data);

                //InspirationAmount++;
                //amountText.text = InspirationAmount.ToString() + "/3";

                //if(InspirationAmount == 3) { explainationText.GetComponent<TextMeshProUGUI>().text = "Ga terug naar de Dome!"; }
                //else { explainationText.SetActive(false); }
                CurrentPOI = null;
                CinematicCameraHandler.Instance.StopFocus();
                return;
            }

            float _distance = Vector3.Distance(player.transform.position, CurrentPOI.transform.position);
            if (_distance > CurrentPOI.FocusDistance)
            {
                explainationText.SetActive(false);
                CurrentPOI = null;
                CinematicCameraHandler.Instance.StopFocus();
                Debug.Log("STOPPING FOCUS");
            }
        }
    }
}
