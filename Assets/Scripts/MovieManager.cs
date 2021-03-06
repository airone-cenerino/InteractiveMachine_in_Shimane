using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;


public class MovieManager : MonoBehaviour
{
	[SerializeField] private Color intervalColor;
	[SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private RawImage rawImageForFadeOut;
	[SerializeField] private RawImage rawImage;
	[SerializeField] private VideoClip[] normalVideoClips;
	[SerializeField] private VideoClip[] specialVideoClips;
	private float intervalTime = 3f;


	private StatusManager statusManager;
	private AudioSource audioSource;
	private int nowPlayingClipNum = 0;
	private Coroutine changeToNormalMovieCoroutine;


	private void Awake()
	{
		statusManager = GetComponent<StatusManager>();
		audioSource = GetComponent<AudioSource>();

		videoPlayer.isLooping = true;
		videoPlayer.clip = normalVideoClips[nowPlayingClipNum];
		videoPlayer.loopPointReached += FinishPlayingVideo;

		// FadeOutの色の初期化。
		rawImageForFadeOut.color = new Color(intervalColor.r, intervalColor.g, intervalColor.b, 0f);
	}


	// 再生が終了した際に呼ばれる。
	public void FinishPlayingVideo(VideoPlayer vp)
	{
		// 特別映像への遷移中に映像が終了しても、次の通常映像へは移行しない。
		if (statusManager.currentStatus != StatusManager.Status.SpecialFadeOut)
		{
			changeToNormalMovieCoroutine = StartCoroutine(ChangeToNormalMovieCoroutine());
		}
	}


	// 特別映像へ切り替える。
	public void ChangeToSpecialMovie(int speialMovieNum)
	{
		// 既に特別映像再生中 or 遷移中であれば何もしない。
		if (statusManager.currentStatus == StatusManager.Status.SpecialPlaying || statusManager.currentStatus == StatusManager.Status.SpecialFadeOut)
		{
			return;
		}

		if (statusManager.currentStatus == StatusManager.Status.NormalInterval)
		{
			try
			{
				StopCoroutine(changeToNormalMovieCoroutine);    // インターバル中断用
			}
			catch (System.NullReferenceException e) { }

			StartCoroutine(ChangeToSpecialMovieCoroutine(speialMovieNum, false));
		}
		else
		{
			StartCoroutine(ChangeToSpecialMovieCoroutine(speialMovieNum, true));
		}
	}

	
	public void SetIntervalTime(float intervalTime)
    {
		this.intervalTime = intervalTime;
		Debug.Log("Interval Timeを" + intervalTime + "秒に変更します。");
	}


	// 通常映像切り替え用コルーチン。
	private IEnumerator ChangeToNormalMovieCoroutine()
	{
		statusManager.ChangeStatus(StatusManager.Status.NormalInterval);
		rawImageForFadeOut.color = new Color(intervalColor.r, intervalColor.g, intervalColor.b, 1f);   // インターバル画面の色にする。
		videoPlayer.Stop();


		// 一定時間待つ。
		yield return new WaitForSeconds(intervalTime);


		// 次の通常映像に切り替え。
		nowPlayingClipNum++;
		if (nowPlayingClipNum >= normalVideoClips.Length)
			nowPlayingClipNum = 0;
		videoPlayer.clip = normalVideoClips[nowPlayingClipNum];
		videoPlayer.Play();

		statusManager.ChangeStatus(StatusManager.Status.NormalPlaying);

		yield return new WaitForSeconds(0.5f);  // 前の動画のキャッシュが消えるのを待つ。

		rawImageForFadeOut.color = new Color(intervalColor.r, intervalColor.g, intervalColor.b, 0f);   // 透明に戻す。
	}


	// 特別映像切り替え用コルーチン。
	private IEnumerator ChangeToSpecialMovieCoroutine(int speialMovieNum, bool isPlaying)
	{
		statusManager.ChangeStatus(StatusManager.Status.SpecialFadeOut);

		// フェードアウト
		videoPlayer.isLooping = false;
		if (isPlaying)
		{
			// 再生中の時はrawImageで暗くする。
			while (rawImageForFadeOut.color.a < 1f)
			{
				yield return null;
				rawImageForFadeOut.color = new Color(rawImageForFadeOut.color.r, rawImageForFadeOut.color.g, rawImageForFadeOut.color.b, rawImageForFadeOut.color.a + Time.deltaTime);
				videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) - Time.deltaTime);
			}
		}
        else
        {
			videoPlayer.Play();
		}


		videoPlayer.isLooping = true;
		videoPlayer.SetDirectAudioVolume(0, 1f);
		videoPlayer.clip = specialVideoClips[speialMovieNum];
		audioSource.Play();

		statusManager.ChangeStatus(StatusManager.Status.SpecialPlaying);

		yield return new WaitForSeconds(0.5f);		// 前の動画のキャッシュが消えるのを待つ。

		rawImageForFadeOut.color = new Color(intervalColor.r, intervalColor.g, intervalColor.b, 0f);    // 透明に戻す。
	}
}
