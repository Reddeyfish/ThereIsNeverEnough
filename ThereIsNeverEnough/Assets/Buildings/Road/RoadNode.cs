using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RoadNode : MonoBehaviour {

    [SerializeField]
    protected GameObject connectionVisuals;

    public int distance { get; set; }

    protected TileLocation location;
    public TileLocation Location { get { return location; } }
    List<OutboundRoad> neighbors = new List<OutboundRoad>();
    List<RoadNode> outbound = new List<RoadNode>();
	// Use this for initialization

	protected virtual void Start () {
        location = GetComponentInParent<AbstractTile>().location;
        distance = int.MaxValue - 10;
        TryAddConnection(location.up());
        TryAddConnection(location.down());
        TryAddConnection(location.left());
        TryAddConnection(location.right());
        RecomputeCosts();
	}

    void TryAddConnection(TileLocation otherLocation)
    {
        if (Terrain.self.validTileLocation(otherLocation))
        {
            RoadNode other = Terrain.self.tiles[otherLocation].GetComponentInChildren<RoadNode>();
            if (other != null)
            {
                GameObject spawnedConnectionVisuals = GameObject.Instantiate(connectionVisuals);
                spawnedConnectionVisuals.transform.SetParent(this.transform, false);
                OutboundRoad newRoad = new OutboundRoad(other, spawnedConnectionVisuals.GetComponent<LineRenderer>());
                newRoad.renderer.SetPosition(1, (Vector2)otherLocation - (Vector2)location);
                neighbors.Add(newRoad);

                other.RecieveConnection(this, newRoad.renderer);
            }
        }
    }

    public void RecieveConnection(RoadNode node, LineRenderer rend)
    {
        neighbors.Add(new OutboundRoad(node, rend));
    }

    void RecomputeCosts()
    {
        bool valueChanged = false;
        foreach (OutboundRoad or in neighbors)
        {
            if (distance > or.node.distance + 1)
            {
                distance = or.node.distance + 1;
                outbound.Clear();
                outbound.Add(or.node);
                valueChanged = true;
            }
            else if (distance == or.node.distance + 1)
            {
                outbound.Add(or.node);
            }
        }

        if (valueChanged)
        {
            foreach (OutboundRoad or in neighbors)
            {
                or.node.RecomputeCosts();
            }
            Debug.Log(distance);
        }
    }

    public virtual void Recieve(Person person)
    {
        if (outbound.Count != 0)
            person.Target = outbound[Random.Range(0, outbound.Count - 1)];
        else
            Debug.Log(neighbors.Count);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    class OutboundRoad
    {
        public readonly RoadNode node;
        public readonly LineRenderer renderer;

        public OutboundRoad(RoadNode node, LineRenderer renderer)
        {
            this.node = node;
            this.renderer = renderer;
        }
    }
}
