using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonManager : MonoBehaviour {
	public GameObject main;
	public GameObject credits;
	public GameObject tutorial;

	void Start() {
		main.SetActive (true);
		credits.SetActive (false);
		tutorial.SetActive (false);
	}

	public void LoadGame() {
		SceneManager.LoadScene ("Main");
	}

	public void Credits() {
		if (main.activeSelf) {
			main.SetActive (false);
			credits.SetActive (true);
		} else {
			main.SetActive (true);
			credits.SetActive (false);
		}

	}

	public void Tutorial() {
		if (main.activeSelf) {
			main.SetActive (false);
			tutorial.SetActive (true);
		} else {
			main.SetActive (true);
			tutorial.SetActive (false);
		}

	}

	public void QuitGame() {
		Application.Quit ();
	}
}
