using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terrain : MonoBehaviour, IObservable<FluidTick> {
    public static Terrain self;

    [SerializeField]
    protected GameObject genericTile;

    [SerializeField]
    protected GameObject mainBasePrefab;
    GameObject spawnedMainBase;

    [SerializeField]
    protected GameObject peoplePrefab;

    [SerializeField]
    protected int worldSize;
    public int WorldSize { get { return worldSize; } }

    [SerializeField]
    protected float flowViscosity;
    [SerializeField]
    protected float evaporationCutoff;

    [SerializeField]
    protected float tickSpeed;

    public Map<AbstractTile> tiles;

    const float noiseMultiplier = 5 * Mathf.PI;
    Vector2 seed;

    Observable<FluidTick> fluidTickObservable;
    public Observable<FluidTick> Observable(IObservable<FluidTick> self)
    {
        return fluidTickObservable;
    }

    Map<float> fluidDeltas;

    void Awake()
    {
        spawnedMainBase = GameObject.Instantiate(mainBasePrefab);
        self = this;
        fluidTickObservable = new Observable<FluidTick>();
        StartCoroutine(fluidTick());
    }

	// Use this for initialization
	void Start () {
        seed = Random.insideUnitCircle * 999;
        tiles = new Map<AbstractTile>(worldSize);
        for (int x = 1 - worldSize; x < worldSize; x++)
        {
            for (int y = 1 - worldSize; y < worldSize; y++)
            {
                TileLocation location = new TileLocation(x, y);
                tiles[location] = SimplePool.Spawn(genericTile, location).GetComponent<AbstractTile>();
                tiles[location].location = location;
#if UNITY_EDITOR
                tiles[location].transform.SetParent(this.transform);
#endif
                tiles[location].Height = Mathf.PerlinNoise(seed.x + location.X / noiseMultiplier, seed.y + location.Y / noiseMultiplier);
            }
        }

        tiles[1 - worldSize][1 - worldSize].gameObject.AddComponent<FluidSpawner>();
        tiles[1 - worldSize][worldSize - 1].gameObject.AddComponent<FluidSpawner>();
        tiles[worldSize - 1][1 - worldSize].gameObject.AddComponent<FluidSpawner>();
        tiles[worldSize - 1][worldSize - 1].gameObject.AddComponent<FluidSpawner>();

        spawnedMainBase.transform.SetParent(tiles[0][0].transform, false);

        for (int i = 0; i < 5; i++)
        {
            GameObject spawnedPeople = GameObject.Instantiate(peoplePrefab);
            TileLocation location = new TileLocation(Random.Range(1-worldSize, worldSize), Random.Range(1-worldSize, worldSize));
            spawnedPeople.transform.SetParent(tiles[location].transform, false);
        }

        fluidDeltas = new Map<float>(worldSize);
	}

    IEnumerator fluidTick()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(tickSpeed);
            fluidTickObservable.Post(new FluidTick());
            doFluidFlow();
        }
    }

    void doFluidFlow()
    {
        for (int x = 1 - worldSize; x < worldSize; x++)
        {
            for (int y = 1 - worldSize; y < worldSize; y++)
            {
                TileLocation location = new TileLocation(x, y);
                AbstractTile tile = tiles[location];
                if (tile.FluidLevel != 0)
                {
                    doFluidDiffs(location);
                }
            }
        }
        applyFluidDiffs();
    }

    void doFluidDiffs(TileLocation location)
    {
        doFluidDiff(location, location.up());
        doFluidDiff(location, location.down());
        doFluidDiff(location, location.left());
        doFluidDiff(location, location.right());
    }

    void doFluidDiff(TileLocation src, TileLocation dest)
    {
        if (validTileLocation(dest))
        {
            float diff = tiles[src].Height + tiles[src].FluidLevel - tiles[dest].Height - tiles[dest].FluidLevel;

            if (tiles[dest].FluidLevel == 0) // new tile
            {
                if (diff > evaporationCutoff)
                {
                    diff *= flowViscosity; //now is actual flow instead of potential
                    fluidDeltas[src] -= diff;
                    fluidDeltas[dest] += diff;
                    tiles[dest].Post(new FluidCovered());
                }
            }
            else if (diff > 0)
            {
                diff *= flowViscosity; //now is actual flow instead of potential
                fluidDeltas[src] -= diff;
                fluidDeltas[dest] += diff;
            }
        }
    }

    void applyFluidDiffs()
    {
        for (int x = 1 - worldSize; x < worldSize; x++)
        {
            for (int y = 1 - worldSize; y < worldSize; y++)
            {
                float newLevel = tiles[x][y].FluidLevel + fluidDeltas[x][y];
                tiles[x][y].FluidLevel = newLevel;
                fluidDeltas[x][y] = 0;
            }
        }
    }

    public bool validTileLocation(TileLocation tile)
    {
        return tile.X > -worldSize && tile.X < worldSize && tile.Y > -worldSize && tile.Y < worldSize;
    }
}

[System.Serializable]
public class TileLocation
{
    readonly int x;
    public int X { get { return x; } }

    readonly int y;
    public int Y { get { return y; } }

	public AbstractTile Tile
	{
		get { return Terrain.self.tiles[this]; }
	}

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

	public TileLocation Adjacent(Directions direction)
	{
		switch (direction)
		{
			case Directions.Down:
				return down();
			case Directions.Up:
				return up();
			case Directions.Left:
				return left();
			case Directions.Right:
				return right();
			default:
				return this;
		}
	}

    public TileLocation up()
    {
        return new TileLocation(x, y + 1);
    }

    public TileLocation down()
    {
        return new TileLocation(x, y - 1);
    }

    public TileLocation left()
    {
        return new TileLocation(x - 1, y);
    }

    public TileLocation right()
    {
        return new TileLocation(x + 1, y);
    }
}

public class FluidTick
{

}

public class Map<T>
{
    private BidirectionalMapArray<T>[] positives;
    private BidirectionalMapArray<T>[] negatives;
    private int size;
    public int Size { get { return size; } }
    public Map(int size)
    {
        this.size = size;
        this.positives = new BidirectionalMapArray<T>[size];
        this.negatives = new BidirectionalMapArray<T>[size - 1];
        for (int i = 0; i < size - 1; i++)
        {
            positives[i] = new BidirectionalMapArray<T>(size);
            negatives[i] = new BidirectionalMapArray<T>(size);
        }
        //and include an extra one because positives is one larger than negatives
        positives[size - 1] = new BidirectionalMapArray<T>(size);
    }

    public BidirectionalMapArray<T> this[int index]
    {
        get
        {
            if (index >= 0)
                return positives[index];
            else
            {
                return negatives[-index - 1];
            }
        }
        set
        {
            if (index >= 0)
                positives[index] = value;
            else
                negatives[-index - 1] = value;
        }
    }

    public T this[TileLocation loc]
    {
        get
        {
            return this[loc.X][loc.Y];
        }
        set
        {
            this[loc.X][loc.Y] = value;
        }
    }
}

public class BidirectionalMapArray<T>
{
    private T[] positives;
    private T[] negatives;
    public BidirectionalMapArray(int size)
    {
        this.positives = new T[size];
        this.negatives = new T[size - 1];
    }

    public T this[int index]
    {
        get
        {
            if (index >= 0)
                return positives[index];
            else
                return negatives[-index - 1];
        }
        set
        {
            if (index >= 0)
                positives[index] = value;
            else
                negatives[-index - 1] = value;
        }
    }
}

