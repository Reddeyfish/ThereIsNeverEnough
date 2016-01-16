using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data for a road on a tile
/// </summary>
public class RoadNode : MonoBehaviour {

	[Tooltip("Road sprite.")]
	public SpriteRenderer RoadSprite;
	[Tooltip("Direction a person will be directed when on this road")]
	public Directions Direction;

    public TileLocation Location { get { return location; } }
    protected TileLocation location;

	protected virtual void Start ()
	{
		var tile = GetComponentInParent<AbstractTile>();
		if (tile != null)
		{
			location = tile.location;
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
}
