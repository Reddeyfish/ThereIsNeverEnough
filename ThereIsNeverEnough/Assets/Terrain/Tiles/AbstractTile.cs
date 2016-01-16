using UnityEngine;
using System.Collections;

public abstract class AbstractTile : MonoBehaviour, IObservable<FluidCovered> {
    public abstract float Height { get; set; }
    public abstract float FluidLevel { get; set; }
    public TileLocation location { get; set; }

    protected Observable<FluidCovered> fluidCoveredObservable;
    public Observable<FluidCovered> Observable(IObservable<FluidCovered> self)
    {
        return fluidCoveredObservable;
    }
}

public class FluidCovered
{
}