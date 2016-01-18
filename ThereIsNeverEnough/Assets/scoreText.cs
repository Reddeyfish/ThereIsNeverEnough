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
		PlayerPrefs.SetInt ("Score", 0);
	}

	void Update () {
        currentScore = mainBase.Score;
		if (currentScore != PlayerPrefs.GetInt("Score")) {
			scoreBox.text = "People Evacuated: " + currentScore;
			PlayerPrefs.SetInt ("Score", currentScore);
		}
	}
}
