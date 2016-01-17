using UnityEngine;
using UnityEngine.UI;
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
            CurrentHealth -= Time.deltaTime;
            if (CurrentHealth < 0)
            {
                Debug.Log("Player Death");
            }
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
}
