using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class MainBase : MonoBehaviour, IObserver<FluidCovered> {

    int score = 0;
    public int Score { get { return score; } }

	// Use this for initialization
	void Start () {

		var tile = GetComponentInParent<AbstractTile>();
		if (tile != null)
		{
			tile.Subscribe(this);
		}
		else
		{
			Debug.LogWarning("Tile in parent main base missing.");
		}
	}

    public void addRescue()
    {
        score++;
        Debug.Log(score);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(Tags.person))
        {
            Person person = other.transform.GetComponent<Person>();
            person.Rescue();
            score++;
            Debug.Log(score);
        }
    }

    public void Notify(FluidCovered fc)
    {
        Debug.Log("Game End!");
        Destroy(this.gameObject);
    }
}
