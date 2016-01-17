using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// Represents a building under construction.
/// </summary>
public class Construction : Building
{

    GameObject completedBuildingPrefab;
    public GameObject CompletedBuildingPrefab
    {
        get { return completedBuildingPrefab; }
        set
        {
            completedBuildingPrefab = value;
            constructionTime = completedBuildingPrefab.GetComponent<Building>().buildTime;
        }
    }

	public GameObject ConstructionCompleteVFX;
	public AudioClip ConstructionStartSFX;
    
    float constructionTime;

    Image image;
    float progress;
    float Progress
    {
        get { return progress; }
        set
        {
            progress = value;
            image.fillAmount = progress / constructionTime;
        }
    }

	private static float TimeSinceLast = 0;

	// Use this for initialization
    void Awake()
    {
        image = GetComponentInChildren<Image>();
    }

	// start the audio
	protected override void Start()
	{
		base.Start();
		PlayConstructionSound();
	}

	private void PlayConstructionSound()
	{
		AudioSource source = Camera.main.GetComponent<AudioSource>();
		if (source != null && Time.time - TimeSinceLast > ConstructionStartSFX.length)
		{
			source.PlayOneShot(ConstructionStartSFX);
			TimeSinceLast = Time.time;
		}
	}

	public void Build()
    {
        Progress += Time.deltaTime;
        if (Progress > constructionTime)
        {
			PlayConstructionSound();
            location.Tile.Building = null;

            Building finishedBuilding = GameObject.Instantiate(CompletedBuildingPrefab).GetComponent<Building>();

            if (finishedBuilding is RoadNode)
            {
                location.Tile.Road = finishedBuilding as RoadNode;
            }
            else
            {
                location.Tile.Building = finishedBuilding;
            }

			Instantiate(ConstructionCompleteVFX, transform.position, Quaternion.identity);
            DestroySelf();
        }
    }

    public override void Notify(FluidCovered message)
    {
        location.Tile.Building = null;
        DestroySelf();
    }

    void DestroySelf()
    {
        location.Tile.Unsubscribe<FluidCovered>(this);
		if (gameObject != null)
		{
			Destroy(gameObject);
		}
    }
}
