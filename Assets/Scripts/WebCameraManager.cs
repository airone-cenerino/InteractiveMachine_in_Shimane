using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WebCameraManager : MonoBehaviour
{
    [SerializeField] StatusManager statusManager;
    [SerializeField] string[] webCamNames;
    [SerializeField] Renderer[] displays;
    private const int CAMERA_WIDTH = 1920;
    private const int CAMERA_HEIGHT = 1080;
    private const int CAMERA_FPS = 10;

    private List<WebCamTexture> webcamTextures = new List<WebCamTexture>();
    private float lastShootTime = 0f;


    private void Start()
    {
        // カメラの名前確認用。
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach(WebCamDevice device in devices)
        {
            Debug.Log(device.name);
        }

        for (int i = 0; i < webCamNames.Length; i++)
        {
            webcamTextures.Add(new WebCamTexture(webCamNames[i], CAMERA_WIDTH, CAMERA_HEIGHT, CAMERA_FPS));
            displays[i].material.mainTexture = webcamTextures[i];
            webcamTextures[i].Play();
        }
    }

    private void Update()
    {
        if (Time.time - lastShootTime > 1f/CAMERA_FPS)
        {
            //Debug.Log(webcamTextures[0].GetPixels(0, 0, CAMERA_WIDTH, CAMERA_HEIGHT)[0]);
            lastShootTime = Time.time;
        }



        if (Input.GetKey(KeyCode.Return))
        {
            statusManager.ChangeToSpecialMovie(1);
        }
    }
}
