using UnityEngine;
using System.Collections;

//reads input and sends it to the action script

[RequireComponent(typeof(Action))]
public class PlayerInput : MonoBehaviour, IObserver<PlayerMovedMessage> {

    Action action;

	// Use this for initialization
	void Awake () {
        action = GetComponent<Action>();
        action.Subscribe(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            action.ConstructRoad(action.location().Tile);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            action.ConstructShield(action.location().Tile);
        if (Input.GetMouseButtonDown(2))
            action.ChangeRoadPrefab();
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        action.direction = direction;
	}

    public void Notify(PlayerMovedMessage m)
    {
        if (Input.GetKey(KeyCode.Alpha1))
            action.ConstructRoad(action.location().Tile);
        if (Input.GetKey(KeyCode.Alpha2))
            action.ConstructShield(action.location().Tile);
    }
}
