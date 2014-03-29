using UnityEngine;
using System.Collections;

public class HUDThrottleScript : MonoBehaviour {
	MechController mController;
	WeaponController wController;
	ProgressBarScript throttleBar;
	public Transform throttleBase;
	public Color overheatColour;

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
	}

}
