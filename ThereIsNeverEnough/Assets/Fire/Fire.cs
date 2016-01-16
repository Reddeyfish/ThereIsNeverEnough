using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fire : MonoBehaviour {
	public float fireFrequency;
	public int worldSize;

	public GameObject firePrefab;

	bool spawning = false;
	float timer = 0;


	Dictionary<TileLocation,bool> locations = new Dictionary<TileLocation,bool>();
	void Start () {
		for (int x = 0; x < worldSize; x++) {
			for (int y = 0; y < worldSize; y++) {
				TileLocation location = new TileLocation(x, y);
				locations [location] = false;
			}
		}

//		Debug.Log ("Locations: ");
//		foreach (TileLocation tiles in locations.Keys) {
//			Debug.Log (tiles.X + "," + tiles.Y);
//		}

	}

	
	// Update is called once per frame
	void Update () {

		if(!spawning){
			timer += Time.deltaTime;
		}
		//when timer reaches 2 seconds, call Spawn function
		if(timer >= fireFrequency){
			Spawn();
		}


	}

	TileLocation GenerateLocation() {
		return new TileLocation (UnityEngine.Random.Range (0, worldSize), UnityEngine.Random.Range (0, worldSize));
	}

	void Spawn() {
		spawning = true;
		timer = 0;

		bool result;
		TileLocation location = GenerateLocation ();
		locations.TryGetValue (location, out result);

//		Debug.Log ("Random wants to spawn at:" + location.X +","+ location.Y);

		if (!(result)) {
			SimplePool.Spawn (firePrefab, location);
		}

		locations [location] = true;

		spawning = false;
	}
}
