using UnityEngine;
using System.Collections;

public class RayBulletScript : MonoBehaviour {

		public float maxRange;
		float hitRange;
		
		public float myDamage;
		public GameObject hitEffect;
		public GameObject tracerObject;
		RaycastHit targetHit;
		bool fired=false;
		public float tracerProbability;
		bool hit;
		
		void Start()
		{
			if(tracerObject)
			{
				//if(Random.Range (0,1)<=tracerProbability)
					Instantiate (tracerObject,transform.position,transform.rotation);

			}
		}


		
		// Update is called once per frame
		void LateUpdate () {
			
			if(fired==false){							
				fired=true;
			}
			else{
			hit=Physics.Raycast (transform.position,transform.rotation*Vector3.forward,out targetHit,maxRange,~(1<<13));
				if(hit){
				
					Instantiate(hitEffect,targetHit.point+targetHit.normal,Quaternion.LookRotation (targetHit.normal));
					
				if(targetHit.collider.transform.CompareTag("DamageObject")){

					targetHit.collider.transform.BroadcastMessage("TakeDamage",new CollisionDataContainer(myDamage,targetHit.point,targetHit.normal));
				}

				}
				Destroy (gameObject);
			}
		}

	void OnDrawGizmos(){
		Gizmos.DrawRay (transform.position,transform.rotation*Vector3.forward*maxRange);
	}

}
