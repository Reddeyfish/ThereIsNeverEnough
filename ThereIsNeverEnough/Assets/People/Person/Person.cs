using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {

    [SerializeField]
    protected float speed;

    RoadNode target;
    public RoadNode Target { get { return target; } set { target = value; } }

    //Road currentRoad

	// Use this for initialization
	void Start () {
	
	}

    public void Rescue()
    {
        SimplePool.Despawn(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if ((target.transform.position - this.transform.position).magnitude < speed * Time.deltaTime)
        {
            target.Receive(this);
        }
        else
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }
}
