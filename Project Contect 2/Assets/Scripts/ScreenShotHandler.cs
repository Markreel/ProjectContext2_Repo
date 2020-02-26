using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotHandler : MonoBehaviour
{
    [SerializeField] GameObject picturePrefab;
    [SerializeField] GameObject hovercraft;

    [SerializeField] private Texture currentPicture;
    public List<Texture> pictures;

    private Camera cam;

    private void Awake()
    {
        SetupCam();
    }

    private void SetupCam()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        //transform.position = Camera.main.transform.position;
        //transform.rotation = Camera.main.transform.rotation;
        //cam.fieldOfView = Camera.main.fieldOfView;

        if (Input.GetButtonDown("Fire1")) { TakePicture(); }
    }

    private void TakePicture()
    {
        //currentPicture = Camera.main.activeTexture;
        //Camera.main.targetTexture.
        //pictures.Add(currentPicture);
        Camera Cam = cam;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;

        Cam.Render();

        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        currentPicture = Image;
        pictures.Add(currentPicture);

        //SpawnPhoto();
    }

    private void SpawnPhoto()
    {
        GameObject _pic = Instantiate(picturePrefab, hovercraft.transform.position + Vector3.up, picturePrefab.transform.rotation);
        _pic.transform.eulerAngles = new Vector3(_pic.transform.eulerAngles.x, hovercraft.transform.GetChild(0).eulerAngles.y + 180, _pic.transform.eulerAngles.z);

        Material _mat = new Material(Shader.Find("Unlit/Texture"));
        _mat.mainTexture = currentPicture;

        _pic.GetComponent<Renderer>().material = _mat;
    }

}
