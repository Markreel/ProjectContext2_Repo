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
    [SerializeField] List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();

    [SerializeField] Sprite KubismeShape;
    [SerializeField] Sprite FauvismeShape;
    [SerializeField] Sprite ExpressionismeShape;
    [SerializeField] Sprite PointillismeShape;

    [SerializeField] Camera artCam;
    [SerializeField] ScreenShotHandler screenShotHandler;

    [SerializeField] Color backgroundColor;
    [SerializeField] int shapeAmount = 5;
    [SerializeField] GameObject shapePrefab;
    [SerializeField] List<GameObject> shapes;
    [SerializeField] GameObject backgroundPlane;

    Coroutine createArtRoutine;
    Coroutine pointillismeRoutine;

    void Start()
    {
        oldShapeAmount = shapeAmount;
        //CreateArtPiece();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //CreateArtPiece(); 
            Pointillism();
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

    IEnumerator IECreateArtPiece()
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

    bool _isHalf = false;
    int oldShapeAmount = 0;

    private void Pointillism()
    {
        if (pointillismeRoutine != null) StopCoroutine(pointillismeRoutine);
        pointillismeRoutine = StartCoroutine(IEPointillism());
    }

    private IEnumerator IEPointillism()
    {
        //shapeAmount = _isHalf ? oldShapeAmount * 2 : oldShapeAmount;
        //_isHalf = _isHalf ? false : true;

        Texture2D _tex = (Texture2D)screenShotHandler.pictures[0];
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
                    _px = BlackWhite(_px);

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

    //Returns the black/white value of the given color
    private Color BlackWhite(Color _col)
    {
        float _highestValue = Mathf.Max(_col.r, _col.g, _col.b);
        _col.r = _col.g = _col.b = _highestValue;

        return _col;
    }
}
