using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Action : MonoBehaviour
{
	public Vector2 direction { get; set; }

	[SerializeField]
	protected float speed;

	[SerializeField]
	protected float accel;

	Rigidbody2D rigid;
	MainBase mainBase;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		mainBase = FindObjectOfType<MainBase>();
	}

	private void FixedUpdate()
	{
		rigid.velocity = Vector2.MoveTowards(rigid.velocity, speed * direction.normalized, accel * Time.fixedDeltaTime);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(Tags.people))
		{
			People people = other.GetComponent<People>();
			people.Active = true;
			people.AddRoad(mainBase.transform);
		}
	}

	public TileLocation location()
	{
		return new TileLocation(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
	}
}
