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


    // �Đ����I�������ۂɌĂ΂��B
    public void FinishPlayingVideo(VideoPlayer vp)
    {
        // ���ʉf���ւ̑J�ڒ��ɉf�����I�����Ă��A���̒ʏ�f���ւ͈ڍs���Ȃ��B
        if (statusManager.currentStatus != StatusManager.Status.SpecialFadeOut)
        {
            changeToNormalMovieCoroutine = StartCoroutine(ChangeToNormalMovieCoroutine());
        }
    }


    // ���ʉf���֐؂�ւ���B
    public void ChangeToSpecialMovie(int speialMovieNum)
    {
        if (statusManager.currentStatus == StatusManager.Status.NormalInterval)
        {
            try
            {
                StopCoroutine(changeToNormalMovieCoroutine);    // �C���^�[�o�����f�p
            }
            catch (System.NullReferenceException e) { }

            StartCoroutine(ChangeToSpecialMovieCoroutine(speialMovieNum, false));
        }
        else
        {
            StartCoroutine(ChangeToSpecialMovieCoroutine(speialMovieNum, true));
        }
    }


    // �ʏ�f���؂�ւ��p�R���[�`���B
    private IEnumerator ChangeToNormalMovieCoroutine()
    {
        statusManager.ChangeStatus(StatusManager.Status.NormalInterval);
        videoPlayer.gameObject.SetActive(false);    // ����ʂɂ���B

        // ��莞�ԑ҂B
        yield return new WaitForSeconds(intervalTime);

        videoPlayer.gameObject.SetActive(true);

        // ���̒ʏ�f���ɐ؂�ւ��B
        nowPlayingClipNum++;
        if (nowPlayingClipNum >= normalVideoClips.Length)
            nowPlayingClipNum = 0;
        videoPlayer.clip = normalVideoClips[nowPlayingClipNum];

        statusManager.ChangeStatus(StatusManager.Status.NormalPlaying);
    }


    // ���ʉf���؂�ւ��p�R���[�`���B
    private IEnumerator ChangeToSpecialMovieCoroutine(int speialMovieNum, bool isPlaying)
    {
        statusManager.ChangeStatus(StatusManager.Status.SpecialFadeOut);

        // �t�F�[�h�A�E�g
        videoPlayer.isLooping = false;
        if (isPlaying)
        {
            // �Đ����̎���rawImage�ňÂ�����B
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
            // �C���^�[�o���̎��̓J�����̔w�i�F�ňÂ�����B
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
