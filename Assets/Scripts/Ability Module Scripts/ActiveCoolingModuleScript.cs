using UnityEngine;
using System.Collections;

public class ActiveCoolingModuleScript : AbilityModuleScript {
	public float coolingPerSecond = 5f;

	// Update is called once per frame
	public override void activateModule () {
		
		if(capacityLevel>0)
			isActive = true;

	}
	
	public override void deactivateModule(){
		isActive = false;
	}
	
	public override void activatedUpdate(){
		if(wControl.heatLevel>0){
			wControl.heatLevel -= coolingPerSecond * Time.deltaTime;
		}
		else{
			deactivateModule ();
		}
	}
}
