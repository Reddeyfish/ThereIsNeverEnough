using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class NormalTile : AbstractTile {

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
