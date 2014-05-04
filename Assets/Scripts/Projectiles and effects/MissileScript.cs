using UnityEngine;
using System.Collections;

public class MissileScript : MonoBehaviour {

	public float initialForce=10000f;
	public float acceleration = 100f;
	public float accelTime=3f;
	public float myDamage;
	public float armPeriod;

	public Vector3 homingPoint;
	public Transform homingTransform;
	
	public Transform impactEffect;
	
	Rigidbody myCollider;
	
	// Use this for initialization
	void Awake(){
		myCollider = GetComponent<Rigidbody>();
	}
	
	void Start () {
		myCollider.AddForce (transform.forward*initialForce);
		armPeriod+=Time.time;
		accelTime+=Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(accelTime>=Time.time){
			myCollider.AddForce(transform.forward*acceleration*Time.deltaTime);
		}
	}

	void FixedUpdate(){
		if(homingTransform!=null){
			homingPoint=homingTransform.position;
		}

		Quaternion relativeTarget = Quaternion.RotateTowards (rigidbody.rotation,Quaternion.LookRotation (homingPoint-rigidbody.position),20f);
		rigidbody.MoveRotation (relativeTarget);
	}
	
	void OnCollisionEnter(Collision collision){

			Instantiate (impactEffect,transform.position,transform.rotation);
			
			if(collision.transform.CompareTag ("DamageObject")){
				collision.transform.BroadcastMessage("TakeDamage",new CollisionDataContainer(myDamage,collision.contacts[0].point, collision.contacts[0].normal));
			}
			
			Destroy (gameObject);

		
	}

	void OnDrawGizmos(){
		//Gizmos.DrawRay(rigidbody.position,homingPoint);
		if(homingTransform!=null){
			Gizmos.DrawWireSphere (homingTransform.position,5f);
		}
	}
}
