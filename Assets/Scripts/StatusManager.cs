using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatusManager : MonoBehaviour
{
    [SerializeField] MovieManager movieManager;
    [HideInInspector] public enum Status { NormalPlaying, NormalInterval, SpecialPlaying, SpecialFadeOut};

    public Status currentStatus { get; private set; } = Status.NormalPlaying;
    public Status lastStatus { get; private set; } = Status.NormalPlaying;


    // 特別映像へ切り替える。
    public void ChangeToSpecialMovie(int speialMovieNum)
    {
        // 既に特別映像再生中 or 遷移中であれば何もしない。
        if(currentStatus == Status.SpecialPlaying || currentStatus == Status.SpecialFadeOut)
        {
            return;
        }

        movieManager.ChangeToSpecialMovie(speialMovieNum);
    }


    // ステータス更新。
    public void ChangeStatus(Status newStatus)
    {
        lastStatus = currentStatus;
        currentStatus = newStatus;
        Debug.Log(currentStatus);
    }
}
