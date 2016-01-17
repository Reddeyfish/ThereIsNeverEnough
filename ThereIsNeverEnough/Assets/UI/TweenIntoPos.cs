using UnityEngine;
using System.Collections;

/// <summary>
/// Tweens this object to the current pos using a tween
/// </summary>
public class TweenIntoPos : MonoBehaviour {

	public Transform StartPos;
	[Tooltip("If bounce is set to true the object will bounce.")]
	public bool Bounce = false;
	[Tooltip("Destination is 10% past where it should be before tweening back.")]
	public bool Overshoot = false;
	[Tooltip("Speed")]
	public float Speed = 2.0f;
	[Tooltip("How we ease the tween.")]
	public iTween.EaseType EaseType = iTween.EaseType.easeInOutQuad;

	public void Start()
	{
		Vector3 destination = transform.position;

		int points = 2;

		if (Bounce)
		{
			points += 4;
		}
		if (Overshoot)
		{
			points += 2;
		}

		Vector3[] path = new Vector3[points];

		path[0] = StartPos.position;
		path[path.Length - 1] = destination;

		if (Bounce)
		{
			// first go to the destination
			path[1] = destination;

			// then go to 50% in between destinations
			path[2] = StartPos.position + (destination - StartPos.position) / 2;
			path[3] = destination;
			path[4] = StartPos.position + (destination - StartPos.position) / 4;
		}

		if (Overshoot)
		{
			path[path.Length - 2] = destination + (destination - StartPos.position) * 0.1f;
		}

		transform.position = StartPos.position;
		iTween.MoveTo(gameObject, iTween.Hash("path", path, "speed", Speed, "easetype", EaseType));
	}
}
