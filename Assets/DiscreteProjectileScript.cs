using UnityEngine;
using System.Collections;

public class DiscreteProjectileScript : MonoBehaviour {

	public float initialForce=10000f;
	public float myDamage;
	public float armPeriod;

	public Transform impactEffect;

	Rigidbody myCollider;

	// Use this for initialization
	void Awake(){
		myCollider = GetComponent<Rigidbody>();
	}

	void Start () {
		myCollider.AddForce (transform.forward*initialForce);
		armPeriod+=Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision){
		//if(armPeriod<=Time.time){
		Instantiate (impactEffect,transform.position,transform.rotation);

		if(collision.transform.CompareTag ("DamageObject")){
			collision.transform.BroadcastMessage("TakeDamage",myDamage);
		}

		Destroy (gameObject);
		//}

	}
}
