using UnityEngine;
using System.Collections;

public class DebrisScript : MonoBehaviour {
	public float lifetime = 10f;
	float deathtime;
	public GameObject flameEffect;
	public float explosionForce = 10000;

	Rigidbody[] childBodies;

	// Use this for initialization
	void Awake () {
		deathtime =Time.time+lifetime;
		childBodies = GetComponentsInChildren<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time>=deathtime)
			Destroy (gameObject);

		for(int r=0; r<childBodies.Length; r++)
		{
			childBodies[r].transform.localScale = Vector3.one*((deathtime-Time.time)/lifetime);
		}

	}

	public void explode(Vector3 explosionPoint)
	{
		for(int r=0; r<childBodies.Length; r++)
		{
			GameObject particleFlame = (GameObject)Instantiate(flameEffect,childBodies[r].transform.position,Quaternion.identity);
			particleFlame.transform.parent = childBodies[r].transform;
			childBodies[r].AddExplosionForce(explosionForce,explosionPoint,50f);
		}
	}
}
