using UnityEngine;
using System.Collections;

//reads input and sends it to the action script

[RequireComponent(typeof(Action))]
public class PlayerInput : MonoBehaviour {

    Action action;

	// Use this for initialization
	void Awake () {
        action = GetComponent<Action>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        action.direction = direction;
	}
}
