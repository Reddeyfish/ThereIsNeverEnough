using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class updateText : MonoBehaviour 
{

	Text updateBox;
	int minute = 0, time = 0;

	// Use this for initialization
	void Start () 
	{
		updateBox = GetComponent <Text> ();
		updateBox.text = "Time: ";
	}

	// Update is called once per frame
	void Update () 
	{
		time = (int)Time.time;
		if (time < 60) 
		{
			updateBox.text = "Time: " + time + " sec";
		} 
		else 
		{
			minute = (int)Time.time / 60;
			time = time - (minute * 60);
			updateBox.text = "Time: " + minute + " min ";
			if (time != 0) 
			{
				updateBox.text += time + " sec";
			}
		}

	}
}
