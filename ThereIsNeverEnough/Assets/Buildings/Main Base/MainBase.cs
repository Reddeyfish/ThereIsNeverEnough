﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class MainBase : RoadNode, IObserver<FluidCovered> {

    int score = 0;
    public int Score { get { return score; } }

	// Use this for initialization
	protected override void Start () {
        location = new TileLocation(0, 0);
        distance = 0;
	}

    public void addRescue()
    {
        score++;
        Debug.Log(score);
    }

    public override void Notify(FluidCovered fc)
    {
        Debug.Log("Game End!");
        base.Notify(fc);
    }

    public override void Recieve(Person person)
    {
        person.Rescue();
        addRescue();
    }
}
