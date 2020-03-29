using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour, IFocusable
{
    public enum Type {Kubisme, Fauvisme, Expressionisme, Pointillisme}
    public Type ArtType;
    public List<Color> ColorPalette;
    [Range(0, 1)] public float Priority;
    public List<Sprite> Shapes;
    [SerializeField] private float focusDistance;
    public float FocusDistance { get { return focusDistance; } set {focusDistance = value; } }
    public ScreenShotHandler ScreenShotHandler;

    public bool IsFinalPOI = false;

    [HideInInspector] public bool Visited = false;
}

public class PointOfInterestData
{
    public PointOfInterest.Type ArtType;
    public List<Color> ColorPalette;
    [Range(0, 1)] public float Priority;
    public List<Sprite> Shapes;
    public Texture2D Picture;
}