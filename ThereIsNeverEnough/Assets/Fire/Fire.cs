using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Fire : MonoBehaviour {
	public float fireFrequency;
	public GameObject firePrefab;
	public Dictionary<TileLocation,bool> locations = new Dictionary<TileLocation,bool>();

	int worldSize;
	bool spawning = false;
	float timer = 0;
	float increaseTimer = 0;
	TileLocation mainBase;


	void Start () {
		worldSize = FindObjectOfType<Terrain> ().WorldSize;
		mainBase = new TileLocation (0, 0);

		for (int x = 1-worldSize; x < worldSize; x++) {
			for (int y = 1-worldSize; y < worldSize; y++) {
				TileLocation location = new TileLocation(x, y);
				locations [location] = false;
			}
		}
	}
		
	// Update is called once per frame
	void Update () {
		increaseTimer += Time.deltaTime;

		if(!spawning){
			timer += Time.deltaTime;
		}

		if(timer >= fireFrequency){
			Spawn(GenerateLocation());
		}

		if (mainBase.Tile.FluidLevel != 0) {
			SceneManager.LoadScene ("Score");
		}
	}

	TileLocation GenerateLocation() {
		return new TileLocation (UnityEngine.Random.Range (1-worldSize, worldSize), UnityEngine.Random.Range (1-worldSize, worldSize));
	}

	void Spawn(TileLocation spawningLocation) {
		spawning = true;
		timer = 0;


		if (spawningLocation.X == 0 && spawningLocation.Y == 0) {
			SceneManager.LoadScene("Score");
		}

		bool result;
		locations.TryGetValue (spawningLocation, out result);
		if (!(result)) {
			SimplePool.Spawn (firePrefab, spawningLocation).GetComponent<Burn> ().location = spawningLocation;
		}



		locations [spawningLocation] = true;
		spawning = false;
	}
}
