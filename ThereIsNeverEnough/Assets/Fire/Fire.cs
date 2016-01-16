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
		for (int x = 1-worldSize; x < worldSize; x++) {
			for (int y = 1-worldSize; y < worldSize; y++) {
				TileLocation location = new TileLocation(x, y);
				locations [location] = false;
			}
		}
	}

	
	// Update is called once per frame
	void Update () {
		if(!spawning){
			timer += Time.deltaTime;
		}

		if(timer >= fireFrequency){
			Spawn();
		}
	}

	TileLocation GenerateLocation() {
		return new TileLocation (UnityEngine.Random.Range (1-worldSize, worldSize), UnityEngine.Random.Range (1-worldSize, worldSize));
	}

	void Spawn() {
		spawning = true;
		timer = 0;

		bool result;
		TileLocation location = GenerateLocation ();
		locations.TryGetValue (location, out result);

		if (!(result)) {
			SimplePool.Spawn (firePrefab, location);
		}

		locations [location] = true;

		spawning = false;
	}
}
