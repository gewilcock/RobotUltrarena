using UnityEngine;
using System.Collections;

public class TracerScript : MonoBehaviour {
	public float speed = 10f;
	public float lifetime=0.2f;
	// Use this for initialization
	void Start () {
		lifetime += Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.forward*speed*Time.deltaTime);
		if(Time.time>lifetime)
			Destroy (gameObject);
	}

	void OnTriggerEnter(Collider collide){

		Destroy (gameObject);
		
	}
}
