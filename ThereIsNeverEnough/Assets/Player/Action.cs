using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Action : MonoBehaviour, IObservable<PlayerMovedMessage> {

    public Vector2 direction { get; set; }
    public bool building;

    [SerializeField]
    protected GameObject contructionPrefab;
    [SerializeField]
    protected GameObject[] roadPrefabs;
    [SerializeField]
    protected GameObject shieldPrefab;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float accel;

    Observable<PlayerMovedMessage> playerMovedObservable = new Observable<PlayerMovedMessage>();
    public Observable<PlayerMovedMessage> Observable(IObservable<PlayerMovedMessage> self) { return playerMovedObservable; }

    Rigidbody2D rigid;

    TileLocation currentLocation;
	private int selectedRoadPrefab = 0;

	private const float changeRoadsDelay = 0.8f;
	private float m_scrollTimer = 0;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentLocation = location();
    }

    void Update()
    {
		// if the player is clicking
		if (Input.GetMouseButton(0))
		{
			// start building a road
            TrySpawnConstruction(roadPrefabs[selectedRoadPrefab], mouseToTileLocation().Tile);
		}

        if (Input.GetMouseButton(1))
        {
            // start building a wall
            TrySpawnConstruction(shieldPrefab, mouseToTileLocation().Tile);
        }

		if (Input.GetAxis("Mouse ScrollWheel") != 0 && m_scrollTimer > changeRoadsDelay)
		{
			m_scrollTimer = 0f;
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (scroll > 0)
			{
				// scroll up
                selectedRoadPrefab = Mathf.Min(selectedRoadPrefab + 1, roadPrefabs.Length - 1);
			}
			else if (scroll < 0)
			{
				// scroll down
				selectedRoadPrefab = Mathf.Max(selectedRoadPrefab - 1, 0);
			}
		}
		m_scrollTimer += Time.deltaTime;

        Building building = currentLocation.Tile.Building;
        if (building != null && building is Construction)
            (building as Construction).Build();
    }

    TileLocation mouseToTileLocation()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new TileLocation(Mathf.RoundToInt(worldPoint.x), Mathf.RoundToInt(worldPoint.y));
    }

    public void ConstructShield()
    {
        TrySpawnConstruction(shieldPrefab);
    }
    public void ConstructRoad()
    {
        TrySpawnConstruction(roadPrefabs[selectedRoadPrefab]);
    }
    /// <summary>
    /// Attempts to spawn a construction.
    /// </summary>
    void TrySpawnConstruction(GameObject completedBuildingPrefab, AbstractTile tile)
    {
        if (tile.FluidLevel != 0)
            return;

        Building building = tile.Building;
        if (building is Construction)
        {
            if(building)
                Destroy(building.gameObject);
            SpawnConstruction(completedBuildingPrefab, tile);
        }
        else if (tile.Road == null && completedBuildingPrefab.GetComponent<Building>() is RoadNode)
        {
            SpawnConstruction(completedBuildingPrefab, tile);
        }
        else if (building == null && !(completedBuildingPrefab.GetComponent<Building>() is RoadNode))
        {
            SpawnConstruction(completedBuildingPrefab, tile);
        }
    }

    void TrySpawnConstruction(GameObject completedBuildingPrefab)
    {
        TrySpawnConstruction(completedBuildingPrefab, currentLocation.Tile);
    }

    /// <summary>
    /// Used by TrySpawnConstruction() to instantiate a construction.
    /// </summary>
    /// <param name="completedBuildingPrefab"></param>
    void SpawnConstruction(GameObject completedBuildingPrefab, AbstractTile tile)
    {
            Construction construction = GameObject.Instantiate(contructionPrefab).GetComponent<Construction>();
            construction.CompletedBuildingPrefab = completedBuildingPrefab;
            tile.Building = construction;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rigid.velocity = Vector2.MoveTowards(rigid.velocity, speed * direction.normalized, accel * Time.fixedDeltaTime);
        TileLocation newLocation = location();
        if (newLocation.X != currentLocation.X || newLocation.Y != currentLocation.Y)
        {
            currentLocation = newLocation;
            playerMovedObservable.Post(new PlayerMovedMessage(currentLocation));
            
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.people))
        {
            CampOfPersons people = other.GetComponent<CampOfPersons>();
            people.Active = true;
        }
    }

    public TileLocation location()
    {
        return new TileLocation(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
    }

    public void ChangeRoadPrefab()
    {
        selectedRoadPrefab = (selectedRoadPrefab + 1) % roadPrefabs.Length;
    }
}

public class PlayerMovedMessage
{
    public readonly TileLocation location;
    public PlayerMovedMessage(TileLocation location)
    {
        this.location = location;
    }
}