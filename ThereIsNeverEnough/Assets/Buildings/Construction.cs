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

	// Use this for initialization
    void Awake()
    {
        image = GetComponentInChildren<Image>();
    }

    public void Build()
    {
        Progress += Time.deltaTime;
        if (Progress > constructionTime)
        {
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

            DestroySelf();
        }
    }

    public override void Notify(FluidCovered message)
    {
        DestroySelf();
    }

    void DestroySelf()
    {
        location.Tile.Unsubscribe<FluidCovered>(this);
        location.Tile.Building = null;
        Destroy(this.gameObject);
    }
}
