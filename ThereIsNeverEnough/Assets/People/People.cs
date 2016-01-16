using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a group of people to be evacuated.
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class People : RoadNode, IObserver<FluidCovered> {

    [SerializeField]
    protected GameObject personPrefab;

    [SerializeField]
    protected Color activeColor;

    bool active = false;
    public bool Active
    {
        set
        {
            active = value;
            if (active)
                minimapVisuals.color = activeColor;
        }
    }
    SpriteRenderer minimapVisuals;
    float timeToNextPerson = 1;

    void Awake()
    {
        minimapVisuals = transform.Find("MinimapVisuals").GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
		AbstractTile tile = GetComponentInParent<AbstractTile>();
		if (tile != null)
		{
			tile.Subscribe(this);
		}
		else
		{
			Debug.LogWarning("Abstract tile in parent people missing.");
		}
    }
		
    void Update()
    {
        if (active)
        {
            timeToNextPerson -= Time.deltaTime;
            if (timeToNextPerson < 0)
            {
                //spawn the next person
                spawnPerson();
                timeToNextPerson += 5f;
            }
        }
    }
		
    void spawnPerson()
    {
        GameObject spawnedPerson = SimplePool.Spawn(personPrefab, this.transform.position);
        Person newPerson = spawnedPerson.GetComponent<Person>();
        Recieve(newPerson);
    }

    public void Notify(FluidCovered fc)
	{
		Destroy (this.gameObject);
	}
}
