using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

	public float flareDimMultiplier=1;
	public float shockLife;
	public float maxDamage;
	//public float damageLifetimeDampening=0.5f;
	public float maxRadius=30f;
	public float maxSphereScale=25f;

	public Transform visibleShock;

	AudioSource mySound;

	float shockDeath;

	ParticleSystem particles;
	Light myLight;
	SphereCollider shockwave;
	// Use this for initialization
	void Start () {
		particles=GetComponent<ParticleSystem>();
		myLight=GetComponent<Light>();
		shockwave = GetComponent<SphereCollider>();
		visibleShock.renderer.material.color=new Vector4(myLight.color.r,myLight.color.g,myLight.color.b,1f);

		shockDeath=Time.time+shockLife;

		mySound = transform.GetComponent<AudioSource>();
		mySound.pitch = Random.Range (0.7f,1.3f);
		mySound.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		float fwoomRatio = (1-((shockDeath-Time.time)/shockLife));

		shockwave.radius=maxRadius;//*fwoomRatio;

		visibleShock.localScale=new Vector3(maxSphereScale*fwoomRatio,maxSphereScale*fwoomRatio,maxSphereScale*fwoomRatio);
		visibleShock.renderer.material.color=new Vector4(myLight.color.r,myLight.color.g,myLight.color.b,(1-fwoomRatio));

		if(shockDeath<=Time.time){shockwave.enabled=false;}


		myLight.intensity=Mathf.Lerp (myLight.intensity,0,Time.deltaTime*flareDimMultiplier);
		myLight.range=Mathf.Lerp (myLight.range,0,Time.deltaTime*flareDimMultiplier);
		if((shockDeath<=Time.time)&&(!particles.IsAlive())){
			Destroy (this.gameObject);
		}
	}

	void OnTriggerEnter(Collider collide){
		if(collide.CompareTag("DamageObject")){

			collide.transform.BroadcastMessage("TakeDamage",new CollisionDataContainer(maxDamage,collide.ClosestPointOnBounds(transform.position),transform.position-collide.transform.position));
		}
	}

	
}
