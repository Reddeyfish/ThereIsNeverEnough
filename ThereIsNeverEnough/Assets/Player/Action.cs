using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Action : MonoBehaviour {

    public Vector2 direction { get; set; }

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float accel;

    [SerializeField]
    protected GameObject roadNodePrefab;

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
	
	// Update is called once per frame
	void FixedUpdate () {
        rigid.velocity = Vector2.MoveTowards(rigid.velocity, speed * direction.normalized, accel * Time.fixedDeltaTime);
        TileLocation newLocation = location();
        if (newLocation.X != currentLocation.X || newLocation.Y != currentLocation.Y)
        {
            currentLocation = newLocation;
            RoadNode currentNode = Terrain.self.tiles[currentLocation].GetComponentInChildren<RoadNode>();
            if (currentNode == null && Terrain.self.tiles[currentLocation].FluidLevel == 0)
            {
                GameObject spawnedRoadNode = GameObject.Instantiate(roadNodePrefab);
				currentLocation.Tile.Road = spawnedRoadNode.GetComponent<RoadNode>();
            }
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
