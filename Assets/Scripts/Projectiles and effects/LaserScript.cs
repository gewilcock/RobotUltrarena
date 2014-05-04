using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour {
	public float lifeTime;
	public float maxRange;
	float hitRange;
	float timeOfDeath;
	public float myDPS;
	GameObject hitLight;
	RaycastHit targetHit;

	AudioSource mySound;

	LineRenderer laserTexture;

	// Use this for initialization
	void Awake () {
		hitLight=transform.FindChild ("Hitlight").gameObject;	
		laserTexture = GetComponent<LineRenderer>();
		timeOfDeath = Time.time+lifeTime;
		mySound=transform.GetComponent<AudioSource>();
	}


	
	// Update is called once per frame
	void LateUpdate () {


		if(Physics.Raycast (transform.position,transform.rotation*Vector3.forward,out targetHit,maxRange,~(1<<13))){
			hitRange=Vector3.Distance (transform.position,targetHit.point);
			laserTexture.SetPosition (1,new Vector3(0f,0f,hitRange));
			hitLight.transform.position=targetHit.point;
			hitLight.transform.rotation=Quaternion.LookRotation (targetHit.normal);
			hitLight.SetActive (true);

			if(targetHit.collider.transform.CompareTag("DamageObject")){
				targetHit.collider.transform.BroadcastMessage("TakeDamage",new CollisionDataContainer(myDPS*Time.deltaTime,targetHit.point,targetHit.normal));
			}

		}
		else{
			hitLight.SetActive (false);
			laserTexture.SetPosition (1,new Vector3(0f,0f,maxRange));
		}

		if(timeOfDeath<=Time.time){
			Destroy (gameObject);
		}

	}
}
