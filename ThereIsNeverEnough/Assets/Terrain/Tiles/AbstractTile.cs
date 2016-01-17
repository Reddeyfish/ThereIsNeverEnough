using UnityEngine;
using System.Collections;

public abstract class AbstractTile : MonoBehaviour, IObservable<FluidCovered> {
    public abstract float Height { get; set; }
    public abstract float FluidLevel { get; set; }
    public TileLocation location { get; set; }

	[Tooltip("Can this tile have a road built on it?")]
	public bool IsRoadBuildable = true;

	/// <summary>
	/// Returns true if this object has a road
	/// </summary>
	public bool HasRoad
	{
		get { return m_road != null; }
	}

	/// <summary>
	/// This tile's road
	/// </summary>
	public RoadNode Road
	{
		get
		{
			return m_road;
		}
		set
		{
			m_road = value;
			m_road.transform.SetParent(transform, false);
		}
	}

    private RoadNode m_road;

    Building building;

    public Building Building
    {
        get
        {
            return building;
        }
        set
        {
            building = value;
            if (building != null)
            {
                building.transform.SetParent(transform, false);
                building.transform.localPosition = Vector3.zero;
            }
        }
    }

    protected Observable<FluidCovered> fluidCoveredObservable;
    public Observable<FluidCovered> Observable(IObservable<FluidCovered> self)
    {
        return fluidCoveredObservable;
    }

	/// <summary>
	/// Returns true if the given road can be built
	/// </summary>
	/// <returns></returns>
	public virtual bool CanBuildRoad(RoadNode road)
	{
		if (FluidLevel != 0)
			return false;

		if (HasRoad && road.RoadStyle == m_road.RoadStyle)
			return false;

		return true;
	}
}

public class FluidCovered
{
    public readonly AbstractTile tile;

    public FluidCovered(AbstractTile tile)
    {
        this.tile = tile;
    }
}