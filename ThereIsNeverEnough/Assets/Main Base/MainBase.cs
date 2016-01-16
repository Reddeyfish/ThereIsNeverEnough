using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class MainBase : MonoBehaviour {

    int score = 0;
    public int Score { get { return score; } }

	// Use this for initialization
	void Start () {

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
}
