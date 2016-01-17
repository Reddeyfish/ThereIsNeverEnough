using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Data for a road on a tile
/// </summary>
public class RoadNode : Building, IObserver<Message>, IObserver<FluidCovered>
{
	[Tooltip("Road sprite.")]
	public SpriteRenderer RoadSprite;
	[Tooltip("Name of the road tileset used")]
	public string RoadStyle;
    
	private bool originNode = false;

    protected int distanceFromMainBase;
    List<Directions> neighbors = new List<Directions>();
    List<Directions> outbound = new List<Directions>();

	private const string TilesetLocation = "Tileset";

    public override float buildTime
    {
        get
        {
            return 0.15f;
        }
    } 

    void Awake()
    {
        Observers.Subscribe(this, RecreatePathsMessage.type);
    }

	protected override void Start () {
        base.Start();
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

				location.Adjacent(direction).Tile.Road.SetSprite();
            }
        }

		SetSprite();

        RecalculatePathing();
	}

	/// <summary>
	/// sets the sprite according to who neighbors this road
	/// </summary>
	private void SetSprite()
	{
		if (string.IsNullOrEmpty(RoadStyle))
			return;

		ConnectionsFlag connect = ConnectionsFlag.None;
		// depending on neighbors we set the sprite
		for (int i = 0; i < neighbors.Count; i++)
		{
			connect |= neighbors[i].ToConnect();
		}

		Sprite[] spritesArray = Resources.LoadAll<Sprite>(TilesetLocation + "/" + RoadStyle);
		switch (connect)
		{
			case ConnectionsFlag.Up | ConnectionsFlag.Down | ConnectionsFlag.Left | ConnectionsFlag.Right:
				SetSpriteRenderer(spritesArray, 9);
				break;

			case ConnectionsFlag.Up | ConnectionsFlag.Down | ConnectionsFlag.Left:
				SetSpriteRenderer(spritesArray, 10);
				break;

			case ConnectionsFlag.Up | ConnectionsFlag.Down | ConnectionsFlag.Right:
				SetSpriteRenderer(spritesArray, 8);
				break;

			case ConnectionsFlag.Up | ConnectionsFlag.Left | ConnectionsFlag.Right:
				SetSpriteRenderer(spritesArray, 13);
				break;

			case ConnectionsFlag.Down | ConnectionsFlag.Left | ConnectionsFlag.Right:
				SetSpriteRenderer(spritesArray, 5);
				break;

			case ConnectionsFlag.Up | ConnectionsFlag.Down:
				SetSpriteRenderer(spritesArray, 7);
				break;

			case ConnectionsFlag.Up | ConnectionsFlag.Left:
				SetSpriteRenderer(spritesArray, 14);
				break;

			case ConnectionsFlag.Up | ConnectionsFlag.Right:
				SetSpriteRenderer(spritesArray, 12);
				break;

			case ConnectionsFlag.Down | ConnectionsFlag.Left:
				SetSpriteRenderer(spritesArray, 6);
				break;

			case ConnectionsFlag.Down | ConnectionsFlag.Right:
				SetSpriteRenderer(spritesArray, 4);
				break;

			case ConnectionsFlag.Left | ConnectionsFlag.Right:
				SetSpriteRenderer(spritesArray, 1);
				break;

			case ConnectionsFlag.Up:
				SetSpriteRenderer(spritesArray, 11);
				break;

			case ConnectionsFlag.Down:
				SetSpriteRenderer(spritesArray, 3);
				break;

			case ConnectionsFlag.Left:
				SetSpriteRenderer(spritesArray, 2);
				break;

			case ConnectionsFlag.Right:
				SetSpriteRenderer(spritesArray, 0);
				break;
		}
	}

	/// <summary>
	/// sets the sprite renderer to a sprite inside the array
	/// </summary>
	/// <param name="spritesArray"></param>
	/// <param name="index"></param>
	private void SetSpriteRenderer(Sprite[] spritesArray, int index)
	{
		Debug.Log("sprite : " + index);
		if (spritesArray.Length > index)
		{
			RoadSprite.sprite = spritesArray[index];
		}
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

	public override void Notify(FluidCovered message)
	{
        base.Notify(message);
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