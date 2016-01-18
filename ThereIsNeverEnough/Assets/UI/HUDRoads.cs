using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDRoads : MonoBehaviour {

	public Action PlayerAction;
	public Sprite[] PossibleRoadSprites;
	public Image HUDSprite;

	private int selectedRoad = 0;

	// Update is called once per frame
	void Update () {
		if (PlayerAction.SelectedRoadTiles != selectedRoad)
		{
			selectedRoad = PlayerAction.SelectedRoadTiles;
			HUDSprite.sprite = PossibleRoadSprites[selectedRoad];
		}
	}
}
