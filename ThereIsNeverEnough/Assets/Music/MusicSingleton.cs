using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicSingleton : MonoBehaviour {

    static MusicSingleton self;

	// Use this for initialization
	void Awake () {
        if (self != null && self != this)
            Destroy(this.gameObject);
        else
        {
            self = this;
            DontDestroyOnLoad(this.gameObject);
        }
	}
}
