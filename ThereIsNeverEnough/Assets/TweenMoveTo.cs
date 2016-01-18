using UnityEngine;
using System.Collections;

public class TweenMoveTo : MonoBehaviour {

	public bool OnStart = true;
	public Vector3 Direction;
	[Tooltip("Speed")]
	public float Speed = 2.0f;
	[Tooltip("How we ease the tween.")]
	public iTween.EaseType EaseType = iTween.EaseType.easeInOutQuad;

	public void Start()
	{
		if(OnStart)
			iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + Direction, "speed", Speed, "easetype", EaseType));
	}
}
