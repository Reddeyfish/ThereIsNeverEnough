using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Action : MonoBehaviour {

    public Vector2 direction { get; set; }

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float accel;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rigid.velocity = Vector2.MoveTowards(rigid.velocity, speed * direction.normalized, accel * Time.fixedDeltaTime);
	}
}
