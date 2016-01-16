using UnityEngine;
using System.Collections;

public abstract class AbstractTile : MonoBehaviour {
    public abstract float Height { get; set; }
    public TileLocation location { get; set; }
}
