﻿using UnityEngine;
using System.Collections;

public class JumpJetsScript : AbilityModuleScript {
	public float maxThrust;
	public float thrustAcceleration;
	public float turnRateBuff = 10;
	public float heatCost = 2;
	float thrustLevel;
	
	void Start(){
		mControl = wControl.myController;
		thrustLevel = 0;
	}
	
	// Update is called once per frame
	public override void activateModule () {
		if((!isActive)&&(!isCharging)){
			isActive = true;
			mControl.JumpMech (0);
			Debug.Log ("firing jumpjets");
			thrustLevel = 0.5f;
			mControl.turnModifier +=turnRateBuff;
		}
	}
	
	public override void deactivateModule(){
		if(isActive){
			Debug.Log ("killing jumpjets");
			isActive = false;
			mControl.JumpMech (thrustLevel);
			mControl.turnModifier -= turnRateBuff;
		}
	}
	
	public override void activatedUpdate(){
		thrustLevel = Mathf.Clamp(thrustLevel+(thrustAcceleration*Time.deltaTime),0,maxThrust);

		wControl.heatLevel += heatCost*Time.deltaTime;
	}
	
}