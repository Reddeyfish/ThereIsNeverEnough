using System.Collections;
using UnityEngine;

/// <summary>
/// Positions box colliders to stop the main camera from panning over the edge of the game
/// </summary>
public class PositionBlockers : MonoBehaviour {

	public GameObject VerticalBlocker;
	public GameObject HorizontalBlocker;

	private const float ColliderOffset = 1.5f;

	// Use this for initialization
	private void Start ()
	{
		StartCoroutine(SetupBarriers());
	}

	/// <summary>
	/// Setup barriers that block the main camera
	/// </summary>
	/// <returns></returns>
	private IEnumerator SetupBarriers()
	{
		yield return new WaitForEndOfFrame();
		Vector3 botLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
		Vector3 topRight = Camera.main.ViewportToWorldPoint(Vector3.one);

		float width = Mathf.Abs(botLeft.x) + Mathf.Abs(topRight.x);
		float height = Mathf.Abs(botLeft.y) + Mathf.Abs(topRight.y);

		Vector3 botLeftTerrain = Terrain.self.tiles[1 - Terrain.self.WorldSize][1 - Terrain.self.WorldSize].transform.position;
		Vector3 topRightTerrain = Terrain.self.tiles[Terrain.self.WorldSize - 1][Terrain.self.WorldSize - 1].transform.position;

		// left wall should be
		GameObject blocker = Instantiate<GameObject>(VerticalBlocker);
		blocker.transform.parent = transform;
		blocker.transform.position = new Vector3(botLeftTerrain.x + width / 2 - ColliderOffset, 0, 0);

		blocker = Instantiate<GameObject>(VerticalBlocker);
		blocker.transform.parent = transform;
		blocker.transform.position = new Vector3(topRightTerrain.x - width / 2 + ColliderOffset, 0, 0);

		blocker = Instantiate<GameObject>(HorizontalBlocker);
		blocker.transform.parent = transform;
		blocker.transform.position = new Vector3(0, topRightTerrain.y - height / 2 + ColliderOffset, 0);

		blocker = Instantiate<GameObject>(HorizontalBlocker);
		blocker.transform.parent = transform;
		blocker.transform.position = new Vector3(0, botLeftTerrain.y + height / 2 - ColliderOffset, 0);
	}
}
