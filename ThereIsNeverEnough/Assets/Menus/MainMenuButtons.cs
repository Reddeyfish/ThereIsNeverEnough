using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains all the functions necessary for the main menu to work
/// </summary>
public class MainMenuButtons : MonoBehaviour {

	public void LoadScene(string scene)
	{
		SceneManager.LoadScene(scene);
	}

	public void QuitApp()
	{
		Application.Quit();
	}
}
