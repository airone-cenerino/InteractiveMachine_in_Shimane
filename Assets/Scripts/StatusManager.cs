using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatusManager : MonoBehaviour
{
	[HideInInspector] public enum Status { NormalPlaying, NormalInterval, SpecialPlaying, SpecialFadeOut};

	public Status currentStatus { get; private set; } = Status.NormalPlaying;
	public Status lastStatus { get; private set; } = Status.NormalPlaying;


	// ステータス更新。
	public void ChangeStatus(Status newStatus)
	{
		lastStatus = currentStatus;
		currentStatus = newStatus;
		Debug.Log(currentStatus);
	}
}
