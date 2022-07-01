using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
	[SerializeField] MovieManager movieManager;
	[SerializeField] string webCamName;
	[SerializeField] float detectThreshold;
	[SerializeField] DetectRectInfo[] detectRects;
	private const int CAMERA_WIDTH = 1920;
	private const int CAMERA_HEIGHT = 1080;
	private const int CAMERA_FPS = 10;
	private const float CAMERA_CALBRATION_TIME = 1f;

	private WebCamTexture webcamTexture;
	private float lastShootTime = 0f;
	private Dictionary<int, List<float>> areaNum2LastColorDict = new Dictionary<int, List<float>>();
	private float elapsedTime = 0f;
	

	private void Start()
	{
		SceneManager.sceneUnloaded += OnSceneUnloaded;

		// カメラの名前確認用。
		WebCamDevice[] devices = WebCamTexture.devices;
		foreach (WebCamDevice device in devices)
		{
			//Debug.Log(device.name);
		}


		// カメラ映像をRawImageに移す設定。
		webcamTexture = new WebCamTexture(webCamName, CAMERA_WIDTH, CAMERA_HEIGHT, CAMERA_FPS);
		display.texture = webcamTexture;
		webcamTexture.Play();


		foreach(DetectRectInfo rectInfo in detectRects)
		{
			// 各検知範囲を赤色でRawImageに示す。
			RectTransform rectTransform = GameObject.Instantiate(detectAreaPanel, canvas.transform).GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(rectInfo.x, -rectInfo.y);
			rectTransform.sizeDelta = new Vector2(rectInfo.width, rectInfo.height);

			// 辞書の初期化
			areaNum2LastColorDict[rectInfo.areaNum] = new List<float>() { -1f, -1f, -1f};
		}
	}


	private void Update()
	{
		// 撮影タイミングの時。
		if (Time.time - lastShootTime > 1f / CAMERA_FPS && CAMERA_CALBRATION_TIME < elapsedTime)
		{
			List<Color> pixelColors = new List<Color>(webcamTexture.GetPixels(0, 0, CAMERA_WIDTH, CAMERA_HEIGHT));  // 画像取得。

			// 対象範囲毎に平均色を確認。
			foreach (DetectRectInfo rectInfo in detectRects)
			{
				float[] currentTargetRectAverageRGB = CalculateTargetRectAverageRGB(pixelColors, rectInfo);


				//Debug.Log("AreaNum:" + rectInfo.areaNum + " 平均RGB:" + currentTargetRectAverageRGB[0].ToString("f2") + " " + currentTargetRectAverageRGB[1].ToString("f2") + " " + currentTargetRectAverageRGB[2].ToString("f2"));
				//Debug.Log("AreaNum:" + rectInfo.areaNum + " RGB変化量:" + CalculateRGBDelta(currentTargetRectAverageRGB, areaNum2LastColorDict[rectInfo.areaNum]).ToString("f2"));

				// 変化量が閾値を超えた時。
				if (CalculateRGBDelta(currentTargetRectAverageRGB, areaNum2LastColorDict[rectInfo.areaNum]) > detectThreshold)
				{
					movieManager.ChangeToSpecialMovie(rectInfo.areaNum);

					Debug.Log("AreaNum:" + rectInfo.areaNum + "の変化量が閾値を超えました。");
				}


				// 最終フレーム色情報更新。
				for (int i = 0; i < 3; i++)
				{
					areaNum2LastColorDict[rectInfo.areaNum][i] = currentTargetRectAverageRGB[i];
				}
			}


			lastShootTime = Time.time;
		}

		elapsedTime += Time.deltaTime;
	}


	// シーン終了時
	void OnSceneUnloaded(Scene scene)
	{
		if (scene.name == "Main")
		{
			webcamTexture.Stop();
		}
	}


	// 画像と対象範囲情報を受け取り、平均RGBを返す。
	private float[] CalculateTargetRectAverageRGB(List<Color> pixelColors, DetectRectInfo rectInfo)
	{
		List<Color> targetPixels = new List<Color>();   // 対象ピクセルの色情報を格納する。

		for (int height = (int)(rectInfo.y - rectInfo.height / 2); height < (int)(rectInfo.y + rectInfo.height / 2); height++)
		{
			targetPixels.AddRange(pixelColors.GetRange((int)((CAMERA_HEIGHT - height - 1) * CAMERA_WIDTH + rectInfo.x - rectInfo.width / 2), rectInfo.width));
		}


		float sumR = 0f, sumG = 0f, sumB = 0f;
		foreach (Color pixel in targetPixels)
		{
			sumR += pixel.r;
			sumG += pixel.g;
			sumB += pixel.b;
		}

		float[] aveRGB = new float[3];
		aveRGB[0] = sumR / targetPixels.Count;
		aveRGB[1] = sumG / targetPixels.Count;
		aveRGB[2] = sumB / targetPixels.Count;

		return aveRGB;
	}


	// RGBの変化量を計算する。
	private float CalculateRGBDelta(float[] currentRGB, List<float> lastRGB)
	{
		// 初期値対応。
		if(lastRGB[0] < 0f)
		{
			return 0f;
		}


		float delta = 0f;

		for (int i = 0; i < 3; i++)
		{
			delta += Mathf.Abs(currentRGB[i] - lastRGB[i]);
		}

		return delta;
	}
}
