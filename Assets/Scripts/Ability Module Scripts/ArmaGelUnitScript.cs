using UnityEngine;
using System.Collections;

public class ArmaGelUnitScript : AbilityModuleScript {
	public float regenPerSecond = 5f;

	void Start(){
		mControl = wControl.myController;
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
		if(mControl.armourLevel < mControl.maxArmour){
			mControl.armourLevel += regenPerSecond * Time.deltaTime;
		}
		else{
			deactivateModule ();
		}
	}
	
}
