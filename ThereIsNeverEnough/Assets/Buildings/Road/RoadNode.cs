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

    int distanceFromMainBase;
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
			location = tile.location;
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
		if (Direction != Directions.None && Terrain.self.validTileLocation(location.Adjacent(Direction)) && location.Adjacent(Direction).Tile.HasRoad)
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

		return location.Adjacent(Directions.None).Tile.Road;
	}

	public virtual void Notify(Message message)
	{
		throw new NotImplementedException();
	}

	public virtual void Notify(FluidCovered message)
	{
        Destroy(this.gameObject);
	}
}

public class RecreatePathsMessage : Message {
    public const string type = "RecreatePathsMessage";
    public RecreatePathsMessage() : base(type) { }
}