using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scoreText : MonoBehaviour {

	Text scoreBox;
	int currentScore;
    MainBase mainBase;

	void Start () {
		scoreBox = GetComponent <Text> ();
		currentScore = 0;
		scoreBox.text = "People Evacuated: " + currentScore;
        mainBase = FindObjectOfType<MainBase>();
	}

	void Update () {
        currentScore = mainBase.Score;
		scoreBox.text = "People Evacuated: " + currentScore;
	}
}
