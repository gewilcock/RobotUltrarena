using UnityEngine;
using System.Collections;

public class ImpactEmitterScript : MonoBehaviour {

	ParticleSystem myEmitter;
	// Use this for initialization
	void Start () {
		myEmitter=GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!myEmitter.IsAlive()){
			Destroy (gameObject);
		}
	}
}
