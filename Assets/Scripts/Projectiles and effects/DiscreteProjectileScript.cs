using UnityEngine;
using System.Collections;

public class DiscreteProjectileScript : MonoBehaviour {

	public float initialForce=10000f;
	public float myDamage;


	public Transform impactEffect;

	Rigidbody myCollider;

	// Use this for initialization
	void Awake(){
		myCollider = GetComponent<Rigidbody>();
	}

	void Start () {
		myCollider.AddForce (transform.forward*initialForce);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision){

		Instantiate (impactEffect,transform.position,transform.rotation);

		if(collision.transform.CompareTag ("DamageObject")){
			collision.transform.BroadcastMessage("TakeDamage",new CollisionDataContainer(myDamage,collision.contacts[0].point, collision.contacts[0].normal));
		}

		Destroy (gameObject);


	}
}
