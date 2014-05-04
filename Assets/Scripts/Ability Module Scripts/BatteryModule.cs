using UnityEngine;
using System.Collections;

public class BatteryModule : AbilityModuleScript {
	public float regenPerSecond = 5f;
	
	void Start(){

	}
	
	// Update is called once per frame
	public override void activateModule () {

		if(capacityLevel>0){
			isActive = true;
		}

	}
	
	public override void deactivateModule(){
		isActive = false;
	}
	
	public override void activatedUpdate(){
		if(wControl.energyLevel<wControl.maxEnergyLevel){
			wControl.energyLevel += regenPerSecond * Time.deltaTime;
		}
		else{
			deactivateModule ();
		}
	}

}
