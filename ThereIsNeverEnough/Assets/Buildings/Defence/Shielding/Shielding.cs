using UnityEngine;
using System.Collections;

public class Shielding : Defence {
    [SerializeField]
    protected int radius;
    [SerializeField]
    protected float height;
	// Use this for initialization
	protected override void Start () {
        base.Start();
        for (int x = location.X + 1 - radius; x < location.X + radius; x++)
        {
            for (int y = location.Y + 1 - radius; y < location.Y + radius; y++)
            {
                AddShieldingHeight(new TileLocation(x, y));
            }
        }
	}

    void AddShieldingHeight(TileLocation l)
    {
        if(Terrain.self.validTileLocation(l))
            Terrain.self.defenceHeightBonuses[l] += ShieldingHeight(l);
    }

    void RemoveShieldingHeight(TileLocation l)
    {
        if (Terrain.self.validTileLocation(l))
            Terrain.self.defenceHeightBonuses[l] -= ShieldingHeight(l);
    }

    float ShieldingHeight(TileLocation l)
    {
        return height * (Mathf.Max(0, radius - Vector2.Distance(l, location))) / radius;
    }

    public override void Notify(FluidCovered message)
    {
        base.Notify(message);
        DestroySelf();
    }

    void DestroySelf()
    {
        for (int x = location.X + 1 - radius; x < location.X + radius; x++)
        {
            for (int y = location.Y + 1 - radius; y < location.Y + radius; y++)
            {
                RemoveShieldingHeight(new TileLocation(x, y));
            }
        }
        Destroy(this.gameObject);
    }
}
