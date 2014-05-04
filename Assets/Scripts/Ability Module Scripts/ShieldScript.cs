using UnityEngine;
using System.Collections;

public class ShieldScript : AbilityModuleScript {

	public float absorptionRatio = 0.5f;

	void Start(){
		mControl = wControl.myController;
	}

	// Update is called once per frame
	public override void activateModule () {

		mControl.isShielded = true;
		mControl.damageDampening = absorptionRatio;
		isActive = true;
	}


	public override void deactivateModule(){
		isActive = false;
		mControl.isShielded = false;
		mControl.damageDampening = absorptionRatio;
	}

}
