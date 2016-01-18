using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {

	[Tooltip("Celebration VFX")]
	public GameObject ConfettiVFX;

    [SerializeField]
    protected float speed;

    RoadNode target;
    public RoadNode Target
    {
        get { return target; }
        set
        {
            if (value != null)
            {
                target = value;
                transform.rotation = ((Vector2)(target.transform.position - (transform.position))).ToRotation();
            }
        }
    }

    public void Rescue()
    {
		Instantiate(ConfettiVFX, transform.position, Quaternion.identity);
        SimplePool.Despawn(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		if (target != null)
		{
			if ((target.transform.position - this.transform.position).magnitude < speed * Time.deltaTime * Target.SpeedMultiplier)
			{
				target.Receive(this);
			}
			else
			{
				this.transform.position = Vector2.MoveTowards(this.transform.position, target.transform.position, speed * Time.deltaTime * Target.SpeedMultiplier);
			}
		}
    }
}
