using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinalScoreDisplay : MonoBehaviour {

	Text scoreBox;

	void Start() {
		scoreBox = GetComponent <Text> ();
		if (PlayerPrefs.GetInt ("Dead") == 0) {
			if (PlayerPrefs.GetInt ("Score") == 0) {
				scoreBox.text = "You alone survive the disaster. The ghosts of the residents of The City of Townsville haunt you.";
			} else if (PlayerPrefs.GetInt ("Score") == 1) {
				scoreBox.text = "You successfully evacuated 1 person. The City of Townsville will rise again.";
			} else {
				scoreBox.text = "You successfully evacuated " + PlayerPrefs.GetInt ("Score") + " people. The City of Townsville will rise again.";
			}
		} else {
			if (PlayerPrefs.GetInt ("Score") == 0) {
				scoreBox.text = "The disaster stuck too quickly. All residents of The City of Townsville perished.";
			} else if (PlayerPrefs.GetInt ("Score") == 1) {
				scoreBox.text = "You successfully evacuated 1 person at the cost of your life. The City of Townsville will rise again.";
			} else {
				scoreBox.text = "You successfully evacuated " + PlayerPrefs.GetInt ("Score") + " people at the cost of your life. The City of Townsville will rise again.";
			}
		}
	}
}
