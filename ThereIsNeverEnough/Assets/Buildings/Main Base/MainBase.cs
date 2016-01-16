using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class MainBase : RoadNode, IObserver<FluidCovered> {

    int score = 0;
    public int Score { get { return score; } }

	// Use this for initialization
	protected override void Start () {
        location = new TileLocation(0, 0);

		var tile = GetComponentInParent<AbstractTile>();
		if (tile != null)
		{
			tile.Subscribe(this);
		}
		else
		{
			Debug.LogWarning("Tile in parent main base missing.");
		}
	}

    public void addRescue()
    {
        score++;
        Debug.Log(score);
    }

    public void Notify(FluidCovered fc)
    {
        Debug.Log("Game End!");
        Destroy(this.gameObject);
    }

    public override void Receive(Person person)
    {
        person.Rescue();
        addRescue();
    }
}
