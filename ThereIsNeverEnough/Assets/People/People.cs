using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a group of people to be evacuated.
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class People : RoadNode, IObserver<FluidCovered> {

    [SerializeField]
    protected GameObject personPrefab;

    [SerializeField]
    protected Color activeColor;

    bool active = false;
    public bool Active
    {
        set
        {
            active = value;
            if (active)
                minimapVisuals.color = activeColor;
        }
    }

    Dictionary<RoadNode, OutgoingRoad> outgoingRoads = new Dictionary<RoadNode, OutgoingRoad>();
    SpriteRenderer minimapVisuals;

    void Awake()
    {
        minimapVisuals = transform.Find("MinimapVisuals").GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
		AbstractTile tile = GetComponentInParent<AbstractTile>();
		if (tile != null)
		{
			tile.Subscribe(this);
		}
		else
		{
			Debug.LogWarning("Abstract tile in parent people missing.");
		}
    }

    public void AddRoad(RoadNode road)
    {
        if(!outgoingRoads.ContainsKey(road))
            outgoingRoads[road] = new OutgoingRoad(this, road);
    }
	
	// Update is called once per frame
	void Update () {
        foreach (OutgoingRoad road in outgoingRoads.Values)
        {
            road.Update();
        }
	}

    void spawnPerson(RoadNode road)
    {
        GameObject spawnedPerson = SimplePool.Spawn(personPrefab, this.transform.position);
        Person newPerson = spawnedPerson.GetComponent<Person>();
        Recieve(newPerson);
    }

    public void Notify(FluidCovered fc)
    {
        Destroy(this.gameObject);
    }

    class OutgoingRoad
    {
        public readonly RoadNode road;
        public float timeToNextPerson = 1;
        People node;

        public OutgoingRoad(People node, RoadNode road)
        {
            this.node = node;
            this.road = road;
        }

        public void Update()
        {
            timeToNextPerson -= Time.deltaTime;
            if (timeToNextPerson < 0)
            {
                //spawn the next person
                node.spawnPerson(road);
                timeToNextPerson += 5f;
            }
        }
    }
}
