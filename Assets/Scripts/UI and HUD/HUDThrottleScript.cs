using UnityEngine;
using System.Collections;

public class HUDThrottleScript : MonoBehaviour {
	MechController mController;
	WeaponController wController;
	ProgressBarScript throttleBar;
	TextMesh speedText;
	public Transform throttleBase;
	public Color overheatColour;

	CharacterController cController;

	float speedTick;

	Color baseColour;

	// Use this for initialization
	void Start() {
		MechInputHandler myHandler=MechInputHandler.playerController;
		
		transform.rotation=Quaternion.LookRotation (transform.position-transform.parent.transform.position);
		transform.Rotate (transform.localEulerAngles.x,0,90f);
		
		mController=myHandler.mControl;
		wController=myHandler.wControl;
		throttleBar=GetComponentInChildren <ProgressBarScript>();
		baseColour=throttleBase.renderer.material.color;

		cController=myHandler.transform.GetComponent<CharacterController>();

		speedText=GetComponentInChildren<TextMesh>();


	}
	
	// Update is called once per frame
	void Update () {

		throttleBar.fillRatio=mController.GetThrottleLevel();

		if(wController.isOverheating){
			throttleBase.renderer.material.color=overheatColour;
		}
		else{
			throttleBase.renderer.material.color=baseColour;
		}

		if(speedTick<Time.time){

			float speed = Mathf.Round (cController.velocity.magnitude)*3.6f;
			speedText.text=speed.ToString ()+" km/h";
			speedTick=Time.time+0.5f;


		}
	}

}
