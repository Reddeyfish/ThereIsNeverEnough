using UnityEngine;
using System.Collections;

public class TweenScale : MonoBehaviour {

	public Vector3 FinalScale = Vector3.one;
	public float Speed = 1.0f;
	public iTween.EaseType EaseType = iTween.EaseType.easeInOutQuad;
	public iTween.LoopType LoopType = iTween.LoopType.none;
	public bool OnStart = true;

	void Start () {
		if (OnStart)
		{
			Scale(gameObject);
		}
	}

	public void Scale(GameObject gameObject)
	{
		iTween.ScaleTo(gameObject, iTween.Hash("scale", FinalScale, "speed", Speed, "easetype", EaseType, "looptype", LoopType));
	}
}
