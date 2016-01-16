using UnityEngine;
using System.Collections;

public abstract class Building : MonoBehaviour, IObserver<FluidCovered>
{
    public TileLocation Location { get { return location; } }
    protected TileLocation location;

    protected virtual void Start()
    {
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
    }

    public virtual void Notify(FluidCovered message)
    {
        message.tile.Unsubscribe<FluidCovered>(this);
    }

}
