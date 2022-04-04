using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WebCameraManager : MonoBehaviour
{
    [System.Serializable]
    public class DetectRectInfo
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public int areaNum;
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
    private Dictionary<int, List<float>> areaNum2LastColorDict = new Dictionary<int, List<float>>();


    private void Start()
    {
        // カメラの名前確認用。
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices)
        {
            Debug.Log(device.name);
        }


        webcamTexture = new WebCamTexture(webCamName, CAMERA_WIDTH, CAMERA_HEIGHT, CAMERA_FPS);
        display.texture = webcamTexture;
        webcamTexture.Play();


        foreach(DetectRectInfo rectInfo in detectRects)
        {
            RectTransform rectTransform = GameObject.Instantiate(detectAreaPanel, canvas.transform).GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectInfo.x, -rectInfo.y);
            rectTransform.sizeDelta = new Vector2(rectInfo.width, rectInfo.height);

            areaNum2LastColorDict[rectInfo.areaNum] = new List<float>() { 0f, 0f, 0f};
        }
    }


    private void Update()
    {
        if (Time.time - lastShootTime > 1f / CAMERA_FPS)
        {
            List<Color> pixelColors = new List<Color>(webcamTexture.GetPixels(0, 0, CAMERA_WIDTH, CAMERA_HEIGHT));  // 画像取得。

            // 対象範囲毎に平均色を確認。
            foreach (DetectRectInfo rectInfo in detectRects)
            {
                List<Color> targetPixels = new List<Color>();

                for (int height = (int)(rectInfo.y - rectInfo.height / 2); height < (int)(rectInfo.y + rectInfo.height / 2); height++)
                {
                    targetPixels.AddRange(pixelColors.GetRange((int)((CAMERA_HEIGHT - height - 1) * CAMERA_WIDTH + rectInfo.x - rectInfo.width / 2), rectInfo.width));
                }


                float sumR = 0f, sumG = 0f, sumB = 0f;
                foreach(Color pixel in targetPixels)
                {
                    sumR += pixel.r;
                    sumG += pixel.g;
                    sumB += pixel.b;
                }

                float aveR = sumR / targetPixels.Count, aveG = sumG / targetPixels.Count, aveB = sumB / targetPixels.Count;

                areaNum2LastColorDict[rectInfo.areaNum][0] = aveR;
                areaNum2LastColorDict[rectInfo.areaNum][1] = aveG;
                areaNum2LastColorDict[rectInfo.areaNum][2] = aveB;


                Debug.Log(webCamName + " AreaNum:" + rectInfo.areaNum + " 平均RGB:" + aveR.ToString("f2") + " " + aveG.ToString("f2") + " " + aveB.ToString("f2"));
            }


            lastShootTime = Time.time;
        }
    }
}