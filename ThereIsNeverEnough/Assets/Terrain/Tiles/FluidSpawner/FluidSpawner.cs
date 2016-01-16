using UnityEngine;
using System.Collections;

public class FluidSpawner : MonoBehaviour, IObserver<FluidTick> {

    AbstractTile tile;

    void Start()
    {
        Terrain.self.Subscribe(this);
        tile = GetComponentInParent<AbstractTile>();
    }

    public void Notify(FluidTick tick)
    {
        tile.FluidLevel += 0.05f;
    }
}
