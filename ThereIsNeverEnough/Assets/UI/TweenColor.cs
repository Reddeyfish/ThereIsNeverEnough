﻿using UnityEngine;
using System.Collections;

public class TweenColor : MonoBehaviour {

	public Color color;
	[Tooltip("Time")]
	public float Time = 1.0f;
	[Tooltip("How we ease the tween.")]
	public iTween.EaseType EaseType = iTween.EaseType.easeInOutQuad;
	public iTween.LoopType LoopType;
	public bool TweenOnStart = true;

	// Use this for initialization
	void Start () {
		if (TweenOnStart)
		{
			iTween.ColorTo(gameObject, iTween.Hash("color", color, "time", Time, "easetype", EaseType, "looptype", LoopType));
		}
	}
}
