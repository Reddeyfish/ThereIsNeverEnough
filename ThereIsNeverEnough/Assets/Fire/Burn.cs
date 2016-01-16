using UnityEngine;
using System.Collections;

public class Burn : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag != "Player") {
			SimplePool.Despawn (collision.gameObject);
		}
	}
}
