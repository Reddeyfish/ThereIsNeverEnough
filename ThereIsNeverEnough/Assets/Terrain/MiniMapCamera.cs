using UnityEngine;
using System.Collections;

public class MiniMapCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Camera>().orthographicSize = GetComponentInParent<Terrain>().WorldSize;
	}
}
