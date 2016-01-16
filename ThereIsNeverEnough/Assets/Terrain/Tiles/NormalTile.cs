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
            visuals.color = HSVColor.HSVToRGB(0.1f, 1f, height);
        }
    }

    SpriteRenderer visuals;



    void Awake()
    {
        visuals = GetComponent<SpriteRenderer>();
    }
}
