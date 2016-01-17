﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Action : MonoBehaviour, IObservable<PlayerMovedMessage> {

    public Vector2 direction { get; set; }
    public bool building;

    [SerializeField]
    protected GameObject contructionPrefab;
    [SerializeField]
    protected GameObject roadPrefab;
    [SerializeField]
    protected GameObject shieldPrefab;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float accel;

    [SerializeField]
    protected RoadNode roadNodePrefab;

    Observable<PlayerMovedMessage> playerMovedObservable = new Observable<PlayerMovedMessage>();
    public Observable<PlayerMovedMessage> Observable(IObservable<PlayerMovedMessage> self) { return playerMovedObservable; }

    Rigidbody2D rigid;

    TileLocation currentLocation;

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
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			TileLocation loc = new TileLocation(Mathf.RoundToInt(worldPoint.x), Mathf.RoundToInt(worldPoint.y));
			BuildRoad(loc);
		}

        Building building = currentLocation.Tile.Building;
        if (building is Construction)
            (building as Construction).Build();
    }

    public void ConstructShield()
    {
        TrySpawnConstruction(shieldPrefab);
    }
    public void ConstructRoad()
    {
        TrySpawnConstruction(roadPrefab);
    }
    /// <summary>
    /// Attempts to spawn a construction.
    /// </summary>
    void TrySpawnConstruction(GameObject completedBuildingPrefab)
    {
        if (currentLocation.Tile.FluidLevel != 0)
            return;

        Building building = currentLocation.Tile.Building;
        if (building is Construction)
        {
            if(building)
                Destroy(building.gameObject);
            SpawnConstruction(completedBuildingPrefab);
        }
        else if(currentLocation.Tile.Road == null && completedBuildingPrefab.GetComponent<Building>() is RoadNode)
        {
            SpawnConstruction(completedBuildingPrefab);
        }
        else if (building == null && !(completedBuildingPrefab.GetComponent<Building>() is RoadNode))
        {
            SpawnConstruction(completedBuildingPrefab);
        }
    }

    /// <summary>
    /// Used by TrySpawnConstruction() to instantiate a construction.
    /// </summary>
    /// <param name="completedBuildingPrefab"></param>
    void SpawnConstruction(GameObject completedBuildingPrefab)
    {
            Construction construction = GameObject.Instantiate(contructionPrefab).GetComponent<Construction>();
            construction.CompletedBuildingPrefab = completedBuildingPrefab;
            location().Tile.Building = construction;
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

	/// <summary>
	/// Create a road at the location if it's possible to create a road
	/// </summary>
	/// <param name="location"></param>
	private void BuildRoad(TileLocation location)
	{
		if (Terrain.self.validTileLocation(location) && !location.Tile.CanBuildRoad(roadNodePrefab))
		{
			GameObject spawnedRoadNode = Instantiate(roadNodePrefab.gameObject);
			location.Tile.Road = spawnedRoadNode.GetComponent<RoadNode>();
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
}

public class PlayerMovedMessage
{
    public readonly TileLocation location;
    public PlayerMovedMessage(TileLocation location)
    {
        this.location = location;
    }
}