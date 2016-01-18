using UnityEngine;
using System.Collections;

public class TweenFade : MonoBehaviour {

	public float alpha;
	[Tooltip("Time")]
	public float Time = 1.0f;
	[Tooltip("How we ease the tween.")]
	public iTween.EaseType EaseType = iTween.EaseType.easeInOutQuad;
	public iTween.LoopType LoopType;
	public bool TweenOnStart = true;

	// Use this for initialization
	void Start()
	{
		if (TweenOnStart)
		{
			iTween.ColorTo(gameObject, iTween.Hash("alpha", alpha, "time", Time, "easetype", EaseType, "looptype", LoopType));
		}
	}
}
