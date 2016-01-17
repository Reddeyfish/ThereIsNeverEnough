﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Image))]
public class HealthBar : MonoBehaviour {

    [SerializeField]
    protected float maxHealth;

    [SerializeField]
    protected Color maxHealthColor;
    [SerializeField]
    protected Color minHealthColor;

    float currentHealth;
    float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            image.fillAmount = currentHealth / maxHealth;
            image.color = Color.Lerp(minHealthColor, maxHealthColor, image.fillAmount);
        }
    }
    Action action;
    Image image;

	// Use this for initialization
	void Awake () {
        image = GetComponent<Image>();
        currentHealth = maxHealth;
	}

    void Start()
    {
        action = GetComponentInParent<Action>();
    }
		
	// Update is called once per frame
	void Update () {
		if (action.location().Tile.FluidLevel != 0)
        {
			DecreaseHealth (1);
        }
        else if (CurrentHealth < maxHealth)
        {
            CurrentHealth = CurrentHealth += Time.deltaTime;
            if (CurrentHealth > maxHealth)
            {
                CurrentHealth = maxHealth;
                image.color = Color.clear;
            }
        }
	}

	public void DecreaseHealth(int rate) {
		AudioSource source = GetComponent<AudioSource>();
		if (source != null && !source.isPlaying)
		{
			source.Play();
			iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("amount", Vector3.one * 0.2f, "time", 0.8f, "name", "screenshake"));
		}

		CurrentHealth -= Time.deltaTime*rate;
		if (CurrentHealth < 0)
		{
			Debug.Log("Player Death");
			PlayerPrefs.SetInt ("Dead", 1);
			SceneManager.LoadScene("Score");
		}
	}

}
