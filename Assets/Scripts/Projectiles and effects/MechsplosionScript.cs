using UnityEngine;
using System.Collections;

public class MechsplosionScript : MonoBehaviour {
	Light flareLight;
	ParticleSystem particles;
	float deathTime;
	public Transform myParent;
	// Use this for initialization
	void Start () {
		deathTime=Time.time+5.09f;
		flareLight = GetComponent<Light>();
		particles=GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		flareLight.range+=2.5f*Time.deltaTime;
		if(myParent!=null){
			transform.position=myParent.position;
			transform.rotation=myParent.rotation;
		}

		if(Time.time>deathTime){
			flareLight.enabled=false;
			if(!particles.IsAlive ()){GameObject.Destroy (this.gameObject);}
		}
	}
}
