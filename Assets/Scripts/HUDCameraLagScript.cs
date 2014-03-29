using UnityEngine;
using System.Collections;

public class HUDCameraLagScript : MonoBehaviour {
	Transform torsoBone;
	Transform mechTransform;
	float prevRotation;
	float newRotation;

	float prevHeight;
	float currentHeight;
	public float heightBobLimits=0.2f;

	public float lagMultiplier;
	float difference;

	// Use this for initialization
	void Start () {
		mechTransform=MechInputHandler.playerController.mControl.transform;
		torsoBone=MechInputHandler.playerController.mControl.torsoBone;
		prevRotation=torsoBone.localRotation.eulerAngles.x-mechTransform.localRotation.eulerAngles.y;
		prevHeight=torsoBone.position.y;

		currentHeight=0;
	}
	
	// Update is called once per frame
	void LateUpdate () {

		newRotation=lagMultiplier*(torsoBone.localRotation.eulerAngles.x-mechTransform.rotation.eulerAngles.y);

		difference = (newRotation-prevRotation);

		Quaternion diffRotation = Quaternion.Euler(new Vector3(0f,difference,0f));

		Quaternion final = Quaternion.Slerp (transform.localRotation,diffRotation,Time.deltaTime);

		transform.localRotation=final;

		prevRotation=newRotation;


		if(torsoBone.position.y>prevHeight){
			currentHeight=Mathf.Lerp (currentHeight,-heightBobLimits,0.01f);

		}
		else if(torsoBone.position.y<prevHeight){
			currentHeight=Mathf.Lerp (currentHeight,heightBobLimits,Time.deltaTime*3f);
		}
		else{
			currentHeight=Mathf.Lerp (currentHeight,0,Time.deltaTime*3f);
		}



		prevHeight=torsoBone.position.y;

		transform.localPosition=new Vector3(0,currentHeight,0);

	}

}
