using UnityEngine;
using System.Collections;

public abstract class Defence : Building {
    public override void Notify(FluidCovered message)
    {
        message.tile.Unsubscribe<FluidCovered>(this);
        message.tile.Building = null;
        Destroy(this.gameObject);
    }
}