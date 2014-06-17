using UnityEngine;
using System.Collections;

public class ImpactEmitterScript : MonoBehaviour {

	ParticleSystem myEmitter;
	AudioSource mySound;
	// Use this for initialization
	void Start () {
		myEmitter=GetComponent<ParticleSystem>();
		mySound = GetComponent<AudioSource>();
		mySound.pitch = Random.Range (0.8f,1.2f);
		mySound.Play ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!myEmitter.IsAlive()){
			Destroy (gameObject);
		}
	}
}
