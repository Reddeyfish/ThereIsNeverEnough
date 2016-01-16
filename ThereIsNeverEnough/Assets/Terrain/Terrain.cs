using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Terrain : MonoBehaviour {
    [SerializeField]
    protected GameObject genericTile;


    [SerializeField]
    protected int worldSize;

    Dictionary<TileLocation, AbstractTile> tiles = new Dictionary<TileLocation, AbstractTile>();

	// Use this for initialization
	void Start () {
        for (int x = 1 - worldSize; x < worldSize; x++)
        {
            for (int y = 1 - worldSize; y < worldSize; y++)
            {
                //SimplePool.Spawn(genericTile, )
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
    int y;
}