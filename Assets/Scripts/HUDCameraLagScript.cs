using UnityEngine;
using System.Collections;

public class HUDCameraLagScript : MonoBehaviour {
	Transform torsoBone;
	Transform mechTransform;
	float prevRotation;
	float newRotation;

	float prevHeight;
	float newheight;

	public float lagMultiplier;
	public float difference;

	// Use this for initialization
	void Start () {
		mechTransform=GameObject.Find ("PlayerMech").GetComponentInChildren<MechInputHandler>().transform;
		torsoBone=GameObject.Find ("PlayerMech").transform.GetComponentInChildren<MechController>().torsoBone;
		prevRotation=torsoBone.localRotation.eulerAngles.x-mechTransform.localRotation.eulerAngles.y;
		prevHeight=mechTransform.position.y;
	}
	
	// Update is called once per frame
	void LateUpdate () {

		newRotation=lagMultiplier*(torsoBone.localRotation.eulerAngles.x-mechTransform.localRotation.eulerAngles.y);

		difference = (newRotation-prevRotation);

		Quaternion diffRotation = Quaternion.Euler(new Vector3(0f,difference,0f));

		Quaternion final = Quaternion.Slerp (transform.localRotation,diffRotation,Time.deltaTime);

		transform.localRotation=final;

		prevRotation=newRotation;




	}
}
