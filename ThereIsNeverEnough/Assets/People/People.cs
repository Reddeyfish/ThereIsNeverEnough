using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a group of people to be evacuated.
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class People : MonoBehaviour {

    [SerializeField]
    protected GameObject personPrefab;

    bool active = false;
    public bool Active { set { active = value; } }

    Dictionary<Transform, OutgoingRoad> outgoingRoads = new Dictionary<Transform, OutgoingRoad>();

    public void AddRoad(Transform road)
    {
        if(!outgoingRoads.ContainsKey(road))
            outgoingRoads[road] = new OutgoingRoad(this, road);
    }
	
	// Update is called once per frame
	void Update () {
        foreach (OutgoingRoad road in outgoingRoads.Values)
        {
            road.Update();
        }
	}

    void spawnPerson(Transform road)
    {
        GameObject spawnedPerson = SimplePool.Spawn(personPrefab, this.transform.position);
        Person newPerson = spawnedPerson.GetComponent<Person>();
        newPerson.Target = road;
    }

    class OutgoingRoad
    {
        public readonly Transform road;
        public float timeToNextPerson = 0;
        People node;

        public OutgoingRoad(People node, Transform road)
        {
            this.node = node;
            this.road = road;
        }

        public void Update()
        {
            timeToNextPerson -= Time.deltaTime;
            if (timeToNextPerson < 0)
            {
                //spawn the next person
                node.spawnPerson(road);
                timeToNextPerson += 5f;
            }
        }
    }
}
