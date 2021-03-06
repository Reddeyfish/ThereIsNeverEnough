﻿using UnityEngine;
using System.Collections;

public class Burn : MonoBehaviour {

	public TileLocation location;

	void Update() {
		if (location.Tile.FluidLevel != 0) {
			SimplePool.Despawn (this.gameObject);
			FindObjectOfType<Fire> ().locations [location] = false;
		}

		if (location.Tile.HasRoad) {
            location.Tile.Road.DestroySelf();
		}

		if (location.Tile.Resource != null) {
			location.Tile.Resource.DestroyMine ();
		}
	}


	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "IgnoreRaycast") {
			SimplePool.Despawn (collision.gameObject);
		}
	}


	void OnTriggerStay2D(Collider2D player) {
		if (player.gameObject.tag == "Player") {
			player.gameObject.GetComponentInChildren<HealthBar> ().DecreaseHealth (3);
		}
	}
}
