using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class NormalTile : AbstractTile {

	[Tooltip("Can this tile have a road build on it?")]
	public bool RoadBuildable = true;

    float height;
    public override float Height
    {
        get
        {
            return height;
        }
        set
        {
            height = value;
            UpdateColor();
        }
    }

    float fluidLevel;
    public override float FluidLevel
    {
        get
        {
            return fluidLevel;
        }
        set
        {
            fluidLevel = value;
            UpdateColor();
        }
    }

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
	public Road Road
	{
		get
		{
			return m_road;
		}
		set
		{
			m_road = value;
			m_road.gameObject.transform.parent = transform;
		}
	}

	private Road m_road;
    SpriteRenderer visuals;

    void UpdateColor()
    {
        Color baseColor = HSVColor.HSVToRGB(0.1f, 1f, height);
        if (fluidLevel != 0)
            visuals.color = Color.Lerp(baseColor, Color.blue, 0.15f + 0.85f * fluidLevel);
        else
            visuals.color = baseColor;
    }

    void Awake()
    {
        visuals = GetComponent<SpriteRenderer>();
        fluidCoveredObservable = new Observable<FluidCovered>();
    }
}
