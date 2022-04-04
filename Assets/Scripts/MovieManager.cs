using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;


public class MovieManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private StatusManager statusManager;
    [SerializeField] float intervalTime = 3f;
    [SerializeField] private VideoClip[] normalVideoClips;
    [SerializeField] private VideoClip[] specialVideoClips;
    private int nowPlayingClipNum = 0;
    private Coroutine changeToNormalMovieCoroutine;


    private void Awake()
    {
        videoPlayer.isLooping = true;
        videoPlayer.clip = normalVideoClips[nowPlayingClipNum];
        videoPlayer.loopPointReached += FinishPlayingVideo;
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


    // 通常映像切り替え用コルーチン。
    private IEnumerator ChangeToNormalMovieCoroutine()
    {
        statusManager.ChangeStatus(StatusManager.Status.NormalInterval);
        videoPlayer.gameObject.SetActive(false);    // 白画面にする。

        // 一定時間待つ。
        yield return new WaitForSeconds(intervalTime);

        videoPlayer.gameObject.SetActive(true);

        // 次の通常映像に切り替え。
        nowPlayingClipNum++;
        if (nowPlayingClipNum >= normalVideoClips.Length)
            nowPlayingClipNum = 0;
        videoPlayer.clip = normalVideoClips[nowPlayingClipNum];

        statusManager.ChangeStatus(StatusManager.Status.NormalPlaying);
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
            while (rawImage.color.r > 0f)
            {
                yield return null;
                rawImage.color = new Color(rawImage.color.r - Time.deltaTime, rawImage.color.g - Time.deltaTime, rawImage.color.b - Time.deltaTime);
                videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) - Time.deltaTime);
            }

            yield return new WaitForSeconds(1f);
            rawImage.color = new Color(1f, 1f, 1f);
        }
        else
        {
            // インターバルの時はカメラの背景色で暗くする。
            while (camera.backgroundColor.r > 0f)
            {
                yield return null;
                camera.backgroundColor = new Color(camera.backgroundColor.r - Time.deltaTime, camera.backgroundColor.g - Time.deltaTime, camera.backgroundColor.b - Time.deltaTime);
                videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) - Time.deltaTime);
            }

            yield return new WaitForSeconds(1f);
            Camera.main.backgroundColor = new Color(1f, 1f, 1f);
            videoPlayer.gameObject.SetActive(true);
        }

        videoPlayer.isLooping = true;
        videoPlayer.SetDirectAudioVolume(0, 1f);
        videoPlayer.clip = specialVideoClips[speialMovieNum];
        statusManager.ChangeStatus(StatusManager.Status.SpecialPlaying);
    }
}
