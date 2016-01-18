using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terrain : MonoBehaviour, IObservable<FluidTick> {
    public static Terrain self;

	public GameObject[] RockSprites;

    [SerializeField]
	[Tooltip("Tile that shows up if you hover over a place that can have roads built.")]
	protected GameObject highlightTile;

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

	private GameObject m_highlightTileInstance;

    const float noiseMultiplier = 5 * Mathf.PI;
    Vector2 seed;

    Observable<FluidTick> fluidTickObservable;
    public Observable<FluidTick> Observable(IObservable<FluidTick> self)
    {
        return fluidTickObservable;
    }

    Map<float> fluidDeltas;
    public Map<float> defenceHeightBonuses;

	private const int TotalRockMineClusters = 75;
	private const float ChanceOfCluster = 0.56f;

    void Awake()
    {
        defenceHeightBonuses = new Map<float>(worldSize);
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

        tiles[0][0].Road = spawnedMainBase.GetComponent<RoadNode>();

        for (int i = 0; i < 5; i++)
		{
			GameObject spawnedPeople = GameObject.Instantiate(peoplePrefab);
			TileLocation location = GetRandomLocation();
			tiles[location].Road = spawnedPeople.GetComponent<RoadNode>();
		}

		for (int i = 0; i < TotalRockMineClusters; i++)
		{
			var location = GetRandomLocation();
			if (location.Tile.Resource != null || location.Tile.Road != null)
			{
				continue;
			}
			GameObject rockThing = Instantiate(RockSprites[Random.Range(0, RockSprites.Length)]);
			location.Tile.Resource = rockThing.GetComponent<MinableResource>();

			if (Random.Range(0f, 1f) < ChanceOfCluster)
				MakeAdjacentRocks(location, Directions.Up);
			if (Random.Range(0f, 1f) < ChanceOfCluster)
				MakeAdjacentRocks(location, Directions.Down);
			if (Random.Range(0f, 1f) < ChanceOfCluster)
				MakeAdjacentRocks(location, Directions.Left);
			if (Random.Range(0f, 1f) < ChanceOfCluster)
				MakeAdjacentRocks(location, Directions.Right);
		}

        fluidDeltas = new Map<float>(worldSize);

        //create outer bounds
        PolygonCollider2D coll = gameObject.AddComponent<PolygonCollider2D>();
        Vector2[] colliderPoints = new Vector2[10];
        colliderPoints[0] = new Vector2(-1 + worldSize + 0.4f, -1 + worldSize + 0.4f);
        colliderPoints[1] = new Vector2(-1 + worldSize + 0.4f, 1 - (worldSize + 0.4f));
        colliderPoints[2] = new Vector2(1 - (worldSize + 0.4f), 1 - (worldSize + 0.4f));
        colliderPoints[3] = new Vector2(1 - (worldSize + 0.4f), -1 + worldSize + 0.4f);
        colliderPoints[4] = colliderPoints[0];
        
        colliderPoints[5] = colliderPoints[0] + Vector2.one;
        colliderPoints[6] = colliderPoints[1] + new Vector2(1, -1);
        colliderPoints[7] = colliderPoints[2] - Vector2.one;
        colliderPoints[8] = colliderPoints[3] + new Vector2(-1, 1);
        colliderPoints[9] = colliderPoints[5];
        
        coll.points = colliderPoints;
	}

	private void MakeAdjacentRocks(TileLocation location, Directions dir)
	{
		if (Terrain.self.validTileLocation(location.Adjacent(dir)) && location.Adjacent(dir).Tile.Resource == null && location.Adjacent(dir).Tile.Road == null)
		{
			location.Adjacent(dir).Tile.Resource = Instantiate(RockSprites[Random.Range(0, RockSprites.Length)]).GetComponent<MinableResource>();
		}
	}

	private TileLocation GetRandomLocation()
	{
		return new TileLocation(Random.Range(1 - worldSize, worldSize), Random.Range(1 - worldSize, worldSize));
	}

	IEnumerator fluidTick()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(tickSpeed);
            try
            {
                fluidTickObservable.Post(new FluidTick());
                doFluidFlow();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
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
            float diff = tiles[src].Height + defenceHeightBonuses[src] + tiles[src].FluidLevel - tiles[dest].Height - defenceHeightBonuses[dest] - tiles[dest].FluidLevel;

            if (tiles[dest].FluidLevel == 0) // new tile
            {
                if (diff > evaporationCutoff)
                {
                    diff *= flowViscosity; //now is actual flow instead of potential
                    fluidDeltas[src] -= diff;
                    fluidDeltas[dest] += diff;
                    tiles[dest].Post(new FluidCovered(tiles[dest]));
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
                tiles[x][y].FluidLevel = newLevel > defenceHeightBonuses[x][y] ? newLevel : 0;
                fluidDeltas[x][y] = 0;
            }
        }
    }

    public bool validTileLocation(TileLocation tile)
    {
        return tile.X > -worldSize && tile.X < worldSize && tile.Y > -worldSize && tile.Y < worldSize;
    }

	public void HighlightTile(TileLocation tileLoc, bool isVisible)
	{
		if (isVisible)
		{
			if (m_highlightTileInstance == null)
			{
				m_highlightTileInstance = SimplePool.Spawn(highlightTile, tileLoc);
				m_highlightTileInstance.transform.SetParent(transform);
			}

			m_highlightTileInstance.transform.position = tileLoc;
		}

		if (m_highlightTileInstance != null)
		{
			m_highlightTileInstance.SetActive(isVisible);
		}
	}
}

[System.Serializable]
public struct TileLocation
{
	[SerializeField]
    readonly int x;
    public int X { get { return x; } }

	[SerializeField]
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

