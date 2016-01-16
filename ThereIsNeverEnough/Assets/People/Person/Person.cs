using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {

    [SerializeField]
    protected float speed;

    Transform target;
    public Transform Target { get { return target; } set { target = value; } }

    //Road currentRoad

	// Use this for initialization
	void Start () {
	
	}

    public void Rescue()
    {
        target.GetComponent<MainBase>().addRescue();
        SimplePool.Despawn(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if ((target.position - this.transform.position).magnitude < speed * Time.deltaTime)
        {
            Rescue();
        }
        else
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime);
        }
    }
}
