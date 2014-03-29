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

	float shockDeath;

	ParticleSystem particles;
	Light myLight;
	SphereCollider shockwave;
	// Use this for initialization
	void Start () {
		particles=GetComponent<ParticleSystem>();
		myLight=GetComponent<Light>();
		shockwave = GetComponent<SphereCollider>();
		visibleShock.renderer.material.color=new Vector4(myLight.color.r,myLight.color.g,myLight.color.b,0.5f);

		shockDeath=Time.time+shockLife;
	}
	
	// Update is called once per frame
	void Update () {
		float fwoomRatio = (1-((shockDeath-Time.time)/shockLife));

		shockwave.radius=maxRadius*fwoomRatio;

		visibleShock.localScale=new Vector3(maxSphereScale*fwoomRatio,maxSphereScale*fwoomRatio,maxSphereScale*fwoomRatio);
		visibleShock.renderer.material.color=new Vector4(myLight.color.r,myLight.color.g,myLight.color.b,(1-fwoomRatio)*0.5f);

		if(shockDeath<=Time.time){shockwave.enabled=false;}


		myLight.intensity=Mathf.Lerp (myLight.intensity,0,Time.deltaTime*flareDimMultiplier);
		myLight.range=Mathf.Lerp (myLight.range,0,Time.deltaTime*flareDimMultiplier);
		if((shockDeath<=Time.time)&&(!particles.IsAlive())){
			Destroy (this.gameObject);
		}
	}

	void OnTriggerEnter(Collider collide){
		if(collide.CompareTag("DamageObject")){

			collide.transform.BroadcastMessage("TakeDamage",maxDamage);
		}
	}

	
}
