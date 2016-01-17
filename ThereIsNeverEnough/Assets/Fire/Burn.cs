using UnityEngine;
using System.Collections;

public class Burn : MonoBehaviour {

	public TileLocation location;

	void Update() {
		if (location.Tile.FluidLevel != 0) {
			SimplePool.Despawn (this.gameObject);
			FindObjectOfType<Fire> ().locations [location] = false;
		}			
	}


	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag != "Player") {
			SimplePool.Despawn (collision.gameObject);
		}
	}


	void OnTriggerStay2D(Collider2D player) {
		if (player.gameObject.tag == "Player") {
			player.gameObject.GetComponentInChildren<HealthBar> ().DecreaseHealth (3);
		}
	}
}
