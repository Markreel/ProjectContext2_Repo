using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotHandler : MonoBehaviour
{
    public Texture CurrentPicture;
    private Camera cam;

    private void Awake()
    {
        SetupCam();
        TakePicture();
    }

    private void SetupCam()
    {
        cam = GetComponent<Camera>();
    }

    public void TakePicture()
    {
        Camera Cam = cam;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;

        Cam.Render();

        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        CurrentPicture = Image;
    }
}
