using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatusManager : MonoBehaviour
{
    [SerializeField] MovieManager movieManager;
    [HideInInspector] public enum Status { NormalPlaying, NormalInterval, SpecialPlaying, SpecialFadeOut};

    public Status currentStatus { get; private set; } = Status.NormalPlaying;
    public Status lastStatus { get; private set; } = Status.NormalPlaying;


    // ���ʉf���֐؂�ւ���B
    public void ChangeToSpecialMovie(int speialMovieNum)
    {
        // ���ɓ��ʉf���Đ��� or �J�ڒ��ł���Ή������Ȃ��B
        if(currentStatus == Status.SpecialPlaying || currentStatus == Status.SpecialFadeOut)
        {
            return;
        }

        movieManager.ChangeToSpecialMovie(speialMovieNum);
    }


    // �X�e�[�^�X�X�V�B
    public void ChangeStatus(Status newStatus)
    {
        lastStatus = currentStatus;
        currentStatus = newStatus;
        Debug.Log(currentStatus);
    }
}
