using UnityEngine;
using System.Collections;

public class MechStomp : MonoBehaviour {
	AudioSource StompSound;
	public float pitchrange = 0.02f;
	// Use this for initialization
	void Awake()
	{
		StompSound = transform.GetComponent<AudioSource>();
	}
	void PlayStomp () {
		StompSound.pitch = Random.Range (1-pitchrange,1+pitchrange);
		StompSound.Play();
	}
			
	// Update is called once per frame
	void Update () {
	
	}
}
