using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float TimeAfterToDestroy = 5.0f;
	public bool DestroyStoppedParticle = true;

	// Use this for initialization
	void Start () {
		StartCoroutine(DestroySelf());
	}

	private IEnumerator DestroySelf()
	{
		ParticleSystem system = GetComponent<ParticleSystem>();
		while (true)
		{
			yield return new WaitForSeconds(TimeAfterToDestroy);

			if (DestroyStoppedParticle && system != null && system.isStopped)
			{
				break;
			}

			if (!DestroyStoppedParticle)
			{
				break;
			}
		}

		Destroy(gameObject);
	}
}
