using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class MainBase : RoadNode {

    int score = 0;
    public int Score { get { return score; } }

	// Use this for initialization
	protected override void Start () {
        distanceFromMainBase = 0;
        location = new TileLocation(0,0);
	}

    public void addRescue()
    {
        score++;
//        Debug.Log(score);
    }

    public override void Notify(FluidCovered fc)
    {
        Debug.Log("Game End!");
		base.Notify(fc);

    }

    public override void Receive(Person person)
    {
        person.Rescue();
        addRescue();
    }
}
