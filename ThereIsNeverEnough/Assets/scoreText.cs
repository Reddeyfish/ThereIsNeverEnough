using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scoreText : MonoBehaviour {

	Text scoreBox;
	int currentScore;

	void Start () {
		scoreBox = GetComponent <Text> ();
		currentScore = 0;
		scoreBox.text = "People Evacuated: " + currentScore;

	}

	void Update () {
		currentScore = FindObjectOfType<MainBase> ().Score;
		scoreBox.text = "People Evacuated: " + currentScore;
	}
}
