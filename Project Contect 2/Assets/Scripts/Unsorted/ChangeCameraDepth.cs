using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraDepth : MonoBehaviour
{
    public void ChangeDepth(float _depth)
    {
        GetComponent<Camera>().depth = _depth;
    }
}
