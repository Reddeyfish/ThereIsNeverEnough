using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Terrain : MonoBehaviour {
    [SerializeField]
    protected GameObject genericTile;


    [SerializeField]
    protected int worldSize;

    Dictionary<TileLocation, AbstractTile> tiles = new Dictionary<TileLocation, AbstractTile>();

    const float noiseMultiplier = 5 * Mathf.PI;
    Vector2 seed;

	// Use this for initialization
	void Start () {
        seed = Random.insideUnitCircle * 999;
        for (int x = 1 - worldSize; x < worldSize; x++)
        {
            for (int y = 1 - worldSize; y < worldSize; y++)
            {
                TileLocation location = new TileLocation(x, y);
                tiles[location] = SimplePool.Spawn(genericTile, location).GetComponent<AbstractTile>();
                tiles[location].Height = Mathf.PerlinNoise(seed.x + location.X / noiseMultiplier, seed.y + location.Y / noiseMultiplier);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

[System.Serializable]
public class TileLocation
{
    int x;
    public int X { get { return x; } }

    int y;
    public int Y { get { return y; } }

    public TileLocation(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2(TileLocation t)
    {
        return new Vector2(t.x, t.y);
    }

    public static implicit operator Vector3(TileLocation t)
    {
        return new Vector3(t.x, t.y, 0);
    }
}