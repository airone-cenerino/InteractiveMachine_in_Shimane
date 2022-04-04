using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WebCameraManager : MonoBehaviour
{
    [System.Serializable]
    public class DetectRectInfo
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }


    [SerializeField] RawImage display;
    [SerializeField] GameObject detectAreaPanel;
    [SerializeField] GameObject canvas;
    [SerializeField] string webCamName;
    [SerializeField] DetectRectInfo[] detectRects;
    private const int CAMERA_WIDTH = 1920;
    private const int CAMERA_HEIGHT = 1080;
    private const int CAMERA_FPS = 10;

    private WebCamTexture webcamTexture;
    private float lastShootTime = 0f;


    private void Start()
    {
        webcamTexture = new WebCamTexture(webCamName, CAMERA_WIDTH, CAMERA_HEIGHT, CAMERA_FPS);
        display.texture = webcamTexture;
        webcamTexture.Play();



        foreach(DetectRectInfo rectInfo in detectRects)
        {
            RectTransform rectTransform = GameObject.Instantiate(detectAreaPanel, canvas.transform).GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectInfo.x, -rectInfo.y);
            rectTransform.sizeDelta = new Vector2(rectInfo.width, rectInfo.height);
        }
    }


    private void Update()
    {
        if (Time.time - lastShootTime > 1f / CAMERA_FPS)
        {
            //Debug.Log(webcamTextures[0].GetPixels(0, 0, CAMERA_WIDTH, CAMERA_HEIGHT)[0]);
            lastShootTime = Time.time;
        }
    }
}