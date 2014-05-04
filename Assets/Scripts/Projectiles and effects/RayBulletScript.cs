using UnityEngine;
using System.Collections;

public class RayBulletScript : MonoBehaviour {

		public float maxRange;
		float hitRange;
		
		public float myDamage;
		public GameObject hitEffect;
		RaycastHit targetHit;
		bool fired=false;
		public float TracerProbability;
		bool hit;
			
		
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
