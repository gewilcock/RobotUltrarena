using UnityEngine;
using System.Collections;

public class ShieldImpactEffectScript : MonoBehaviour {
	float myAlpha = 0.5f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		myAlpha -= Time.deltaTime;

		if(myAlpha <= 0)
			Destroy (gameObject);
	}
}
