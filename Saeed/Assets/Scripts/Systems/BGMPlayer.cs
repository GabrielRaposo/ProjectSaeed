using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour {

    static public BGMPlayer instance;
    static AudioSource audioSource;
	
	void Awake () {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        } else {
            Destroy(gameObject);
        }
    }

    static public void Play()
    {
        if(!audioSource.isPlaying) audioSource.Play();
    }

    static public void Stop()
    {
        audioSource.Stop();
    }
}
