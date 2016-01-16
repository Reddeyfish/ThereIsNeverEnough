using UnityEngine;
using System.Collections;

public abstract class AbstractTile : MonoBehaviour {
    public abstract float Height { get; set; }
    public abstract float FluidLevel { get; set; }
    public TileLocation location { get; set; }

}
