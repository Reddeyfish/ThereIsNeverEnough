using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Data for a road on a tile
/// </summary>
public class RoadNode : MonoBehaviour, IObserver<Message>, IObserver<FluidCovered>
{
	[Tooltip("Road sprite.")]
	public SpriteRenderer RoadSprite;
	[Tooltip("Direction a person will be directed when on this road")]
	public Directions Direction;

    public TileLocation Location { get { return location; } }
    protected TileLocation location;
    bool originNode = false;

    protected int distanceFromMainBase;
    List<Directions> neighbors = new List<Directions>();
    List<Directions> outbound = new List<Directions>();

    void Awake()
    {
        Observers.Subscribe(this, RecreatePathsMessage.type);
    }

	protected virtual void Start () {
        AbstractTile tile = GetComponentInParent<AbstractTile>();
        if (tile)
        {
            location = tile.location;
            tile.Subscribe<FluidCovered>(this);
        }
        else
        {
            Debug.LogError("missing tile in parent");
        }

        distanceFromMainBase = int.MaxValue - 10;
        
        //add neighbors
        for (int i = 1; i < 5; ++i)
        {
            Directions direction = (Directions)i;
            if (direction == Directions.None)
                continue;

            if (Terrain.self.validTileLocation(location.Adjacent(direction)) && location.Adjacent(direction).Tile.HasRoad)
            {
                neighbors.Add(direction);
                location.Adjacent(direction).Tile.Road.neighbors.Add(direction.inverse());
            }
        }

        RecalculatePathing();
	}

    /// <summary>
    /// Reads the distance of adjacent nodes, sets the lowest as the outbound direction and updates the current distance
    /// </summary>
    void RecalculatePathing()
    {
        bool valueChanged = false;
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (distanceFromMainBase > location.Adjacent(neighbors[i]).Tile.Road.distanceFromMainBase + 1)
            {
                distanceFromMainBase = location.Adjacent(neighbors[i]).Tile.Road.distanceFromMainBase + 1;
                outbound.Clear();
                outbound.Add(neighbors[i]);
                valueChanged = true;
            }
            else if (distanceFromMainBase > location.Adjacent(neighbors[i]).Tile.Road.distanceFromMainBase + 1)
            {
                outbound.Add(neighbors[i]);
            }
        }
        if (valueChanged)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (location.Adjacent(neighbors[i]).Tile.Road.distanceFromMainBase > distanceFromMainBase + 1)
                    location.Adjacent(neighbors[i]).Tile.Road.RecalculatePathing();
            }
        }
    }
    /// <summary>
    /// If a node could have been part of a removed path, reset it's pathing data.
    /// </summary>
    /// <param name="d"></param>
    void RemovePath(Directions d)
    {
        if (outbound.Contains(d))
        {
            outbound.Clear();
            distanceFromMainBase = int.MaxValue - 10;
            for (int i = 0; i < neighbors.Count; i++)
            {
                location.Adjacent(neighbors[i]).Tile.Road.RemovePath(neighbors[i].inverse());
            }
        }
        else
        {
            originNode = true; //this node is the origin node of a new path to avoid the removed node
        }
    }

	/// <summary>
	/// Take the person and throw him along your direction
	/// </summary>
	/// <param name="person"></param>
	public virtual void Receive(Person person)
	{
		person.Target = GetRandomAdjacentNode();
	}

	/// <summary>
	/// Returns a random adjacent node
	/// The direction of this road overrides the return value if possible
	/// </summary>
	private RoadNode GetRandomAdjacentNode()
	{
        if (outbound.Count > 0)
        {
            return location.Adjacent(outbound[UnityEngine.Random.Range(0, outbound.Count - 1)]).Tile.Road;
        }
        else
        {
            return this;
        }
		/* if (Terrain.self.validTileLocation(location.Adjacent(Direction)) && location.Adjacent(Direction).Tile.HasRoad)
			return location.Adjacent(Direction).Tile.Road;

		for (int i = 1; i < 5; ++i)
		{
			Directions direction = (Directions)i;
			if (direction == Directions.None)
				continue;

			if (Terrain.self.validTileLocation(location.Adjacent(direction)) && location.Adjacent(direction).Tile.HasRoad)
            {
                Debug.Log(direction);
				return location.Adjacent(direction).Tile.Road;
			}
		}

		return location.Adjacent(Directions.None).Tile.Road; */
	}

	public virtual void Notify(Message message)
	{
        if (message is RecreatePathsMessage && originNode)
        {
            originNode = false;
            for (int i = 0; i < neighbors.Count; i++)
            {
                location.Adjacent(neighbors[i]).Tile.Road.RecalculatePathing();
            }
        }
	}

	public virtual void Notify(FluidCovered message)
	{
        DestroySelf();
	}

    void DestroySelf()
    {
        //remove self from linkings
        for (int i = 0; i < neighbors.Count; i++)
        {
            location.Adjacent(neighbors[i]).Tile.Road.neighbors.Remove(neighbors[i].inverse());
        }
        //remove self from pathings
        for (int i = 0; i < neighbors.Count; i++)
        {
            location.Adjacent(neighbors[i]).Tile.Road.RemovePath(neighbors[i].inverse());
        }
        Observers.Unsubscribe(this, RecreatePathsMessage.type);
        //recreate pathings
        Observers.Post(new RecreatePathsMessage());
        Destroy(this.gameObject);
    }
}

public class RecreatePathsMessage : Message {
    public const string type = "RecreatePathsMessage";
    public RecreatePathsMessage() : base(type) { }
}