using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RoadNode : MonoBehaviour, IObserver<Message>, IObserver<FluidCovered>
{

    [SerializeField]
    protected GameObject connectionVisuals;

    public int distance { get; set; }

    protected TileLocation location;
    public TileLocation Location { get { return location; } }
    List<OutboundRoad> neighbors = new List<OutboundRoad>();
    List<RoadNode> outbound = new List<RoadNode>();
	// Use this for initialization
    bool originNode = false;

    void Awake()
    {
        Observers.Subscribe(this, RecreatePathsMessage.type);
    }

	protected virtual void Start () {
        location = GetComponentInParent<AbstractTile>().location;
        distance = int.MaxValue - 10;
        TryAddConnection(location.up(), Direction.UP);
        TryAddConnection(location.down(), Direction.DOWN);
        TryAddConnection(location.left(), Direction.LEFT);
        TryAddConnection(location.right(), Direction.RIGHT);
        RecomputeCosts();
        AbstractTile tile = GetComponentInParent<AbstractTile>();
        if (tile)
        {
            tile.Subscribe<FluidCovered>(this);
        }
        else
        {
            Debug.LogError("missing tile in parent");
        }
	}

    void TryAddConnection(TileLocation otherLocation, Direction direction)
    {
        if (Terrain.self.validTileLocation(otherLocation))
        {
            RoadNode other = Terrain.self.tiles[otherLocation].GetComponentInChildren<RoadNode>();
            if (other != null)
            {
                GameObject spawnedConnectionVisuals = GameObject.Instantiate(connectionVisuals);
                spawnedConnectionVisuals.transform.SetParent(this.transform, false);
                OutboundRoad newRoad = new OutboundRoad(other, spawnedConnectionVisuals.GetComponent<LineRenderer>(), direction);
                newRoad.renderer.SetPosition(1, (Vector2)otherLocation - (Vector2)location);
                newRoad.renderer.sortingLayerName = Tags.Layers.road;
                neighbors.Add(newRoad);

                other.RecieveConnection(this, newRoad.renderer, getMirror(direction));
            }
        }
    }

    void RecieveConnection(RoadNode node, LineRenderer rend, Direction direction)
    {
        neighbors.Add(new OutboundRoad(node, rend, direction));
    }

    public void RemoveConnection(RoadNode node)
    {
        for(int i = 0; i < neighbors.Count; i++)
        {
            if(neighbors[i].node == node)
            {
                neighbors.RemoveAt(i);
                break;
            }
        }
    }

    public void RouteDestroyed(RoadNode node)
    {
        if (outbound.Contains(node))
        {
            outbound.Clear();
            distance = int.MaxValue - 10;
        }
        else
        {
            //we are to be the origin node for a new route
            originNode = true;
        }
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
        }
    }

    public virtual void Recieve(Person person)
    {
        if (outbound.Count != 0)
            person.Target = outbound[Random.Range(0, outbound.Count - 1)];
        else
            Debug.Log(neighbors.Count);
    }

    public void Notify(Message r)
    {
        if (r is RecreatePathsMessage)
        {
            if (originNode)
            {
                RecomputeCosts();
                foreach (OutboundRoad or in neighbors)
                {
                    or.node.RecomputeCosts();
                }
                originNode = false;
            }
        }
    }

    public virtual void Notify(FluidCovered f)
    {
        foreach(OutboundRoad outbound in neighbors)
        {
            outbound.node.RemoveConnection(this);
            Destroy(outbound.renderer.gameObject);
        }

        foreach (OutboundRoad outbound in neighbors)
        {
            outbound.node.RouteDestroyed(this);
        }

        //destroy self
        Observers.Unsubscribe(this, RecreatePathsMessage.type);
        Observers.Post(new RecreatePathsMessage());
        Destroy(this.gameObject);
    }

    class OutboundRoad
    {
        public readonly RoadNode node;
        public readonly LineRenderer renderer;
        public readonly Direction direction;

        public OutboundRoad(RoadNode node, LineRenderer renderer, Direction direction)
        {
            this.node = node;
            this.renderer = renderer;
            this.direction = direction;
        }
    }
    enum Direction
    { UP, DOWN, LEFT, RIGHT}
    Direction getMirror(Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                return Direction.DOWN;
            case Direction.DOWN:
                return Direction.UP;
            case Direction.LEFT:
                return Direction.RIGHT;
            case Direction.RIGHT:
                return Direction.LEFT;
        }
        return Direction.UP;
    }
}

public class RecreatePathsMessage : Message {
    public const string type = "RecreatePathsMessage";
    public RecreatePathsMessage() : base(type) { }
}