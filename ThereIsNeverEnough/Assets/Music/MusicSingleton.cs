using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicSingleton : MonoBehaviour {

    static MusicSingleton self;

    public AudioClip menuClip;

    public AudioClip gameClip;

    AudioSource source;

	// Use this for initialization
	void Awake () {
        if (self != null && self != this)
            Destroy(this.gameObject);
        else
        {
            source = GetComponent<AudioSource>();
            self = this;
            DontDestroyOnLoad(this.gameObject);
        }
	}

    void OnLevelWasLoaded(int level)
    {
        source.clip = level == 0 ? menuClip : gameClip;
        source.Play();
        /*
        if (level == 0)
        {
            source.clip = menuClip;
        }
        else
        {
            source.clip = gameClip;
        }*/
    }
}
