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

    void Awake()
    {
        Observers.Subscribe(this, RecreatePathsMessage.type);
    }

	protected virtual void Start () {
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
		if (Terrain.self.validTileLocation(location.Adjacent(Direction)) && location.Adjacent(Direction).Tile.HasRoad)
			return location.Adjacent(Direction).Tile.Road;

		for (int i = 1; i < 5; ++i)
		{
			Directions direction = (Directions)i;
			if (direction == Directions.None)
				continue;

			if (Terrain.self.validTileLocation(location.Adjacent(direction)) && location.Adjacent(direction).Tile.HasRoad)
			{
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
		Debug.Log("Not implemented");
	}
}

public class RecreatePathsMessage : Message {
    public const string type = "RecreatePathsMessage";
    public RecreatePathsMessage() : base(type) { }
}