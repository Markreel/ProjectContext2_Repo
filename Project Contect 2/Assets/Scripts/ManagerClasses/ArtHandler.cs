using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Misschien is t lijp om, los van het schilder aspect, een sculpter aspect toe te voegen. We kunnen dan heel simpel de mesh van poi objecten pakken en deze
/// aan de hand van de UV map animeren omhoog. Ook kunnen we hier een lit shadertje of andere materialtje over gooien waardoor t klei achtig lijkt.
/// 
/// check ff of shader te doen is
/// 
/// Fuck around met shapes (driehoek, ster, ring, etc.)
/// </summary>

public class ArtHandler : MonoBehaviour
{
    public static ArtHandler Instance;

    [SerializeField] List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();
    [SerializeField] List<PointOfInterestData> pointsOfInterestData = new List<PointOfInterestData>();
    private PointOfInterestData currentPOIData;

    [SerializeField] Sprite KubismeShape;
    [SerializeField] Sprite FauvismeShape;
    [SerializeField] Sprite ExpressionismeShape;
    [SerializeField] Sprite PointillismeShape;

    [SerializeField] Camera artCam;
    [SerializeField] ScreenShotHandler screenShotHandler;
    [SerializeField] GameObject backgroundPlane;
    [SerializeField] GameObject shapePrefab;
    List<GameObject> shapes = new List<GameObject>();
    [SerializeField] Material museumMaterial;

    [Header("Settings: ")]
    [SerializeField] List<GameObject> WallPaintings = new List<GameObject>();
    [SerializeField] Color backgroundColor;
    [SerializeField] int shapeAmount = 5;
    [SerializeField] bool isBlackWhite = false;
    [SerializeField] Texture2D testTexture;

    Coroutine createArtRoutine;
    Coroutine pointillismeRoutine;
    Coroutine combineRoutine;
    Coroutine generateArtPieceRoutine;

    private void Awake()
    {
        Instance = Instance ?? this;
        //if(POIHandler.Instance.PointsOfInterestData.Count > 0) { pointsOfInterestData = POIHandler.Instance.PointsOfInterestData; }
    }

    private void Start()
    {
        //Combine();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //        Combine();
    //}

    private void DisplayArtPieces()
    {
        foreach (var _painting in POIHandler.Instance.Paintings)
        {

        }
    }

    private void HandleShapes()
    {
        foreach (var _shape in shapes)
        {
            Destroy(_shape);
        }
        shapes.Clear();

        for (int i = 0; i < shapeAmount; i++)
        {
            GameObject _obj = Instantiate(shapePrefab, transform.parent);
            shapes.Add(_obj);
        }
    }

    private void CreateArtPiece()
    {
        if (createArtRoutine != null) StopCoroutine(createArtRoutine);
        createArtRoutine = StartCoroutine(IECreateArtPiece());
    }

    private IEnumerator IECreateArtPiece()
    {
        HandleShapes();


        PointOfInterest _highestPoi = new PointOfInterest();
        foreach (var _poi in pointsOfInterest)
        {
            _highestPoi = _poi.Priority > _highestPoi.Priority ? _poi : _highestPoi;
        }

        artCam.backgroundColor = _highestPoi.ColorPalette[Random.Range(0, _highestPoi.ColorPalette.Count)];

        //switch (_highestPoi.ArtType)
        //{
        //    default:
        //    case PointOfInterest.Type.Kubisme:
        //        foreach (var _shape in shapes)
        //        {
        //            SpriteRenderer _sr = _shape.GetComponent<SpriteRenderer>();
        //            _sr.sprite = KubismeShape;
        //        }
        //        break;
        //    case PointOfInterest.Type.Fauvisme:
        //        foreach (var _shape in shapes)
        //        {
        //            SpriteRenderer _sr = _shape.GetComponent<SpriteRenderer>();
        //            _sr.sprite = FauvismeShape;
        //        }
        //        break;
        //    case PointOfInterest.Type.Expressionisme:
        //        foreach (var _shape in shapes)
        //        {
        //            SpriteRenderer _sr = _shape.GetComponent<SpriteRenderer>();
        //            _sr.sprite = ExpressionismeShape;
        //        }
        //        break;
        //}

        foreach (var _shape in shapes)
        {
            SpriteRenderer _sr = _shape.GetComponent<SpriteRenderer>();
            _sr.sprite = _highestPoi.Shapes[Random.Range(0, _highestPoi.Shapes.Count)];
            _sr.color = _highestPoi.ColorPalette[Random.Range(0, _highestPoi.ColorPalette.Count)];

            _shape.transform.localScale = Vector3.one * Random.Range(0.5f, 1.25f);
            _shape.transform.position = new Vector3(Random.Range(95f, 105f), 0.5f, Random.Range(-5f, 5f));
            _shape.transform.localEulerAngles = new Vector3(90, 0, Random.Range(0, 360));
        }


        yield return null;
    }

    private void Pointillism()
    {
        if (pointillismeRoutine != null) StopCoroutine(pointillismeRoutine);
        pointillismeRoutine = StartCoroutine(IEPointillism());
    }

    private IEnumerator IEPointillism()
    {
        Texture2D _tex = (Texture2D)screenShotHandler.CurrentPicture;
        artCam.backgroundColor = Color.white;

        foreach (var _shape in shapes)
        {
            Destroy(_shape);
        }
        shapes.Clear();

        backgroundPlane.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        backgroundPlane.SetActive(false);

        float _threshHold = 0.35f;
        List<Vector2Int> _closedPixelsList = new List<Vector2Int>();

        float _grayValue = 1;
        while (_grayValue > -0.25f)
        {
            foreach (var _shape in shapes)
            {
                Destroy(_shape);
            }
            shapes.Clear();

            _grayValue -= Time.deltaTime / 3; //Duration of animation

            for (int x = 0; x <= shapeAmount; x++)
            {
                for (int y = 0; y <= shapeAmount; y++)
                {
                    if (_closedPixelsList.Contains(new Vector2Int(x, y))) { Debug.Log("het werkt"); continue; } //CheckOfMoetSkippen

                    Color _px = _tex.GetPixel(_tex.width / shapeAmount * x, _tex.height / shapeAmount * y);
                    if (isBlackWhite) { _px = BlackWhite(_px); }

                    //Skip pixel aan de hand van de grijswaarde
                    if (_px.grayscale < _grayValue) { continue; }
                    //else if (_px.grayscale - _grayValue > _threshHold) { Debug.Log(_grayValue - _px.grayscale); _closedPixelsList.Add(new Vector2Int(x, y)); }
                    //else { yield return null; }

                    GameObject _obj = Instantiate(shapePrefab, transform.parent);
                    shapes.Add(_obj);

                    Vector3 _offset = new Vector3(Random.Range(-5f / shapeAmount, 5f / shapeAmount), 0, Random.Range(-5f / shapeAmount, 5f / shapeAmount));
                    //Debug.Log(_offset);
                    _obj.transform.position = new Vector3(95 + (10f / shapeAmount * x), 0.5f, -5f + (10f / shapeAmount * y)) + _offset;
                    _obj.transform.localScale = Vector3.one * 1.2f / shapeAmount;

                    SpriteRenderer _sr = _obj.GetComponent<SpriteRenderer>();
                    _sr.sprite = PointillismeShape;

                    _sr.color = _px;

                    //_sr.color = new Color(_px.b, _px.r, _px.g); //SHIFT COLORS

                    //_sr.color =  _tex.GetPixel(_tex.width / shapeAmount * x, _tex.height / shapeAmount * y);
                    //yield return null;
                }
                //yield return null;
                //yield return new WaitForSeconds(0.1f / shapeAmount);
            }
            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }

    private void Combine()
    {
        if (combineRoutine != null) StopCoroutine(combineRoutine);
        combineRoutine = StartCoroutine(IECombine());
    }

    private IEnumerator IECombine()
    {
        //float _threshHold = 0.35f;

        foreach (var _shape in shapes)
        {
            Destroy(_shape);
        }
        shapes.Clear();

        backgroundPlane.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        backgroundPlane.SetActive(false);

        Texture2D _finalTexture = new Texture2D(pointsOfInterestData[0].Picture.width, pointsOfInterestData[0].Picture.height);

        //Create a single texture from all the given textures
        foreach (var _poi in pointsOfInterestData)
        {
            Texture2D _tex = (Texture2D)_poi.Picture;
            for (int x = 0; x <= _tex.width; x++)
            {
                for (int y = 0; y <= _tex.height; y++)
                {
                    Color _pixel = _tex.GetPixel(x, y);

                    if (_pixel != Color.clear) { _finalTexture.SetPixel(x, y, _pixel); }
                }
            }
        }

        foreach (var _shape in shapes)
        {
            Destroy(_shape);
        }
        shapes.Clear();


        List<Vector2Int> _closedPixelsList = new List<Vector2Int>();

        float _grayValue = 1;
        bool _wait = false;
        Color _previousColor = Color.white;

        while (_grayValue > 0f)
        {
            foreach (var _shape in shapes)
            {
                Destroy(_shape);
            }
            shapes.Clear();

            _grayValue -= Time.deltaTime / 10; //Duration of animation

            for (int x = 0; x <= shapeAmount; x++)
            {
                for (int y = 0; y <= shapeAmount; y++)
                {
                    if (_closedPixelsList.Contains(new Vector2Int(x, y))) { continue; } //CheckOfMoetSkippen

                    Color _px = _finalTexture.GetPixel(_finalTexture.width / shapeAmount * x, _finalTexture.height / shapeAmount * y);
                    //if(_px == Color.clear) { continue; }
                    if (isBlackWhite) { _px = BlackWhite(_px); }

                    float _h, _s, _v;
                    Color.RGBToHSV(_px, out _h, out _s, out _v);


                    //Skip pixel aan de hand van de grijswaarde
                    //if (_px.grayscale < _grayValue - 0.25f) { _wait = false; continue; }
                    if (_s < _grayValue - 0.25f) { if (!_wait) { continue; } _px = _previousColor; }
                    else { _wait = true; }
                    _previousColor = _px;

                    //else { Debug.Log(_px.grayscale); }
                    //else { _closedPixelsList.Add(new Vector2Int(x, y)); }
                    //else { yield return null; }

                    GameObject _obj = Instantiate(shapePrefab, transform.parent);
                    shapes.Add(_obj);

                    Vector3 _offset = new Vector3(Random.Range(-5f / shapeAmount, 5f / shapeAmount), 0, Random.Range(-5f / shapeAmount, 5f / shapeAmount));
                    //Debug.Log(_offset);
                    _obj.transform.position = new Vector3(95 + (10f / shapeAmount * x), 0.5f, -5f + (10f / shapeAmount * y)) + _offset;
                    _obj.transform.localScale = Vector3.one * 1.2f / shapeAmount;

                    SpriteRenderer _sr = _obj.GetComponent<SpriteRenderer>();
                    _sr.sprite = pointsOfInterest[0].Shapes[0];

                    _sr.color = _px;
                }
            }
            if (_wait) { yield return new WaitForSeconds(0.001f); }
        }

        POIHandler.Instance.Paintings.Add(SaveArtPiece());
        //UIManager.Instance.FadeOut(3);
        //AudioManager.Instance.PlayOutroClip(); //ALLEEN IN NARRATIVE
        //LevelManager.Instance.LoadScene(0); //ALLEEN IN SANDBOX

        //_threshHold = 1f;
        yield return null;

    }

    public void GenerateArtPiece()
    {
        if (generateArtPieceRoutine != null) StopCoroutine(generateArtPieceRoutine);
        generateArtPieceRoutine = StartCoroutine(IEGenerateArtPiece(currentPOIData));
    }

    private IEnumerator IEGenerateArtPiece(PointOfInterestData _poiData)
    {
        if (POIHandler.Instance.CurrentPOI.IsFinalPOI) { shapeAmount = 30; }

        foreach (var _shape in shapes)
        {
            Destroy(_shape);
        }
        shapes.Clear();

        backgroundPlane.SetActive(true);
        //yield return new WaitForSeconds(0.5f);
        //backgroundPlane.SetActive(false);

        //Texture2D _finalTexture = new Texture2D(_poiData.Picture.width, _poiData.Picture.height);
        Texture2D _finalTexture = (Texture2D)_poiData.Picture;

        //check ff of dit werkt zo en anders pixels goedzetten (kijk naar combine)

        foreach (var _shape in shapes)
        {
            Destroy(_shape);
        }
        shapes.Clear();

        List<Vector2Int> _closedPixelsList = new List<Vector2Int>();

        float _grayValue = 1;
        bool _wait = false;
        Color _previousColor = Color.white;

        while (_grayValue > 0f)
        {
            foreach (var _shape in shapes)
            {
                Destroy(_shape);
            }
            shapes.Clear();

            _grayValue -= Time.deltaTime / 15; //Duration of animation

            for (int x = 0; x <= shapeAmount; x++)
            {
                for (int y = 0; y <= shapeAmount; y++)
                {
                    if (_closedPixelsList.Contains(new Vector2Int(x, y))) { continue; } //CheckOfMoetSkippen

                    Color _px = _finalTexture.GetPixel(_finalTexture.width / shapeAmount * x, _finalTexture.height / shapeAmount * y);

                    if (isBlackWhite) { _px = BlackWhite(_px); }

                    float _h, _s, _v;
                    Color.RGBToHSV(_px, out _h, out _s, out _v);

                    //Skip pixel aan de hand van de grijswaarde
                    if (_s < _grayValue - 0.25f) { if (!_wait) { continue; } _px = _previousColor; }
                    else { _wait = true; }
                    _previousColor = _px;

                    GameObject _obj = Instantiate(shapePrefab, transform.parent);
                    shapes.Add(_obj);

                    Vector3 _offset = new Vector3(Random.Range(-5f / shapeAmount, 5f / shapeAmount), 0, Random.Range(-5f / shapeAmount, 5f / shapeAmount));

                    _obj.transform.position = new Vector3(95 + (10f / shapeAmount * x), 0.5f, -5f + (10f / shapeAmount * y)) + _offset;
                    _obj.transform.localScale = Vector3.one * 1.2f / shapeAmount;

                    SpriteRenderer _sr = _obj.GetComponent<SpriteRenderer>();
                    _sr.sprite = _poiData.Shapes[0];

                    _sr.color = _px;
                }
            }

            testTexture = SaveArtPiece();
            backgroundPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", testTexture);

            if (_wait) { yield return new WaitForSeconds(0.001f); }
        }

        foreach (var _shape in shapes)
        {
            Destroy(_shape);
        }
        shapes.Clear();

        Texture2D _finalPiece = SaveArtPiece();

        bool _isFinalPOI = POIHandler.Instance.CurrentPOI.IsFinalPOI;
        if (!_isFinalPOI)
        {

            UIManager.Instance.FadeOut(3);
            yield return new WaitForSeconds(3);

            backgroundPlane.GetComponent<Renderer>().material.mainTexture = null;

            POIHandler.Instance.Paintings.Add(_finalPiece);

            POIHandler.Instance.DeactivatePointOfInterest();

            CameraController.Instance.ActivateDefaultPlayerCamera();
            UIManager.Instance.SetActiveLetterbox(false);

            yield return new WaitForSeconds(1);
            UIManager.Instance.FadeIn(3);
        }

        else
        {
            museumMaterial.SetTexture("_MainTex", _finalPiece);
            UIManager.Instance.SetActiveLetterbox(false);
            AudioManager.Instance.PlayMusicClip(null);
            yield return new WaitForSeconds(3);
            CameraController.Instance.StartMuseumSwitch();
            POIHandler.Instance.DeactivatePointOfInterest();
        }

        yield return null;

    }


    private Texture2D SaveArtPiece()
    {
        RenderTexture _currentRT = RenderTexture.active;
        RenderTexture.active = artCam.targetTexture;

        artCam.Render();

        Texture2D _returnImage = new Texture2D(artCam.targetTexture.width, artCam.targetTexture.height);

        _returnImage.ReadPixels(new Rect(0, 0, artCam.targetTexture.width, artCam.targetTexture.height), 0, 0);
        _returnImage.Apply();
        RenderTexture.active = _currentRT;

        return _returnImage;
    }

    //private IEnumerator IECombine()
    //{
    //    //float _threshHold = 0.35f;

    //    foreach (var _shape in shapes)
    //    {
    //        Destroy(_shape);
    //    }
    //    shapes.Clear();

    //    backgroundPlane.SetActive(true);
    //    yield return new WaitForSeconds(0.5f);
    //    backgroundPlane.SetActive(false);

    //    Texture2D _finalTexture = new Texture2D(pointsOfInterest[0].screenShotHandler.CurrentPicture.width, pointsOfInterest[0].screenShotHandler.CurrentPicture.height);

    //    //Create a single texture from all the given textures
    //    foreach (var _poi in pointsOfInterest)
    //    {
    //        Texture2D _tex = (Texture2D)_poi.screenShotHandler.CurrentPicture;
    //        for (int x = 0; x <= _tex.width; x++)
    //        {
    //            for (int y = 0; y <= _tex.height; y++)
    //            {
    //                Color _pixel = _tex.GetPixel(x, y);

    //                if (_pixel != Color.clear) { _finalTexture.SetPixel(x, y, _pixel); }
    //            }
    //        }
    //    }

    //    foreach (var _poi in pointsOfInterest)
    //    {
    //        //if(pointsOfInterest.IndexOf(_poi) != pointsOfInterest.Count - 1) { continue; }
    //        Texture2D _tex = (Texture2D)_poi.screenShotHandler.CurrentPicture;
    //        //artCam.backgroundColor = backgroundColor;

    //        foreach (var _shape in shapes)
    //        {
    //            Destroy(_shape);
    //        }
    //        shapes.Clear();


    //        List<Vector2Int> _closedPixelsList = new List<Vector2Int>();

    //        float _grayValue = 1;
    //        bool _wait = false;
    //        Color _previousColor = Color.white;

    //        while (_grayValue > 0f)
    //        {
    //            foreach (var _shape in shapes)
    //            {
    //                Destroy(_shape);
    //            }
    //            shapes.Clear();

    //            _grayValue -= Time.deltaTime / 10; //Duration of animation

    //            for (int x = 0; x <= shapeAmount; x++)
    //            {
    //                for (int y = 0; y <= shapeAmount; y++)
    //                {
    //                    if (_closedPixelsList.Contains(new Vector2Int(x, y))) { continue; } //CheckOfMoetSkippen

    //                    Color _px = _tex.GetPixel(_tex.width / shapeAmount * x, _tex.height / shapeAmount * y);
    //                    //if(_px == Color.clear) { continue; }
    //                    if (isBlackWhite) { _px = BlackWhite(_px); }

    //                    //Skip pixel aan de hand van de grijswaarde
    //                    //if (_px.grayscale < _grayValue - 0.25f) { _wait = false; continue; }
    //                    if (_px.grayscale < _grayValue - 0.25f) { if (!_wait) { continue; } _px = _previousColor; }
    //                    else { _wait = true; }
    //                    _previousColor = _px;

    //                    //else { Debug.Log(_px.grayscale); }
    //                    //else { _closedPixelsList.Add(new Vector2Int(x, y)); }
    //                    //else { yield return null; }

    //                    GameObject _obj = Instantiate(shapePrefab, transform.parent);
    //                    shapes.Add(_obj);

    //                    Vector3 _offset = new Vector3(Random.Range(-5f / shapeAmount, 5f / shapeAmount), 0, Random.Range(-5f / shapeAmount, 5f / shapeAmount));
    //                    //Debug.Log(_offset);
    //                    _obj.transform.position = new Vector3(95 + (10f / shapeAmount * x), 0.5f, -5f + (10f / shapeAmount * y)) + _offset;
    //                    _obj.transform.localScale = Vector3.one * 1.2f / shapeAmount;

    //                    SpriteRenderer _sr = _obj.GetComponent<SpriteRenderer>();
    //                    _sr.sprite = _poi.Shapes[0];

    //                    _sr.color = _px;
    //                }
    //            }
    //            if (_wait) { yield return new WaitForSeconds(0.001f); }
    //        }

    //        //_threshHold = 1f;
    //        yield return null;
    //    }
    //}


    //Returns the black/white value of the given color
    private Color BlackWhite(Color _col)
    {
        float _highestValue = Mathf.Max(_col.r, _col.g, _col.b);
        _col.r = _col.g = _col.b = _highestValue;

        return _col;
    }

    public void UpdatePOIData(PointOfInterestData _currentData, List<PointOfInterestData> _dataList)
    {
        currentPOIData = _currentData;
        pointsOfInterestData = _dataList;
    }
}
