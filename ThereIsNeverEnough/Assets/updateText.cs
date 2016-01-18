using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class updateText : MonoBehaviour 
{

	Text updateBox;

	// Use this for initialization
	void Start () 
	{
		updateBox = GetComponent <Text> ();
		updateBox.text = "Time: " + (int) Time.time + " sec";
	}
	
	// Update is called once per frame
	void Update () 
	{
		updateBox.text = "Time: " + (int) Time.time + " sec";
	}
}
