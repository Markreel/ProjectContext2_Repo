using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    public enum Type {Kubisme, Fauvisme, Expressionisme, Pointillisme}
    public Type ArtType;
    public List<Color> ColorPalette;
    [Range(0, 1)] public float Priority;
    public List<Sprite> Shapes;
}