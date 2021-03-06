﻿using UnityEngine;
using System.Collections;

public class FluidSpawner : MonoBehaviour, IObserver<FluidTick> {

    AbstractTile tile;

    float tickAmount = 0.03f;

    void Start()
    {
        Terrain.self.Subscribe(this);
        tile = GetComponentInParent<AbstractTile>();
    }

    public void Notify(FluidTick tick)
    {
        tile.FluidLevel += tickAmount;
        tickAmount *= 1.0005f;
    }
}
