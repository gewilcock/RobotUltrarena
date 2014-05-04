using UnityEngine;
using System.Collections;

public class AbilityModuleScript : MonoBehaviour {

	public bool isToggledModule;
	public WeaponController wControl;
	public MechController mControl;
	public string moduleName = "Module";
	public float energyCost = 0;
	public bool isActive{get; set;}
	public float abilityCapacity;
	public float capacityLevel;
	public float capacityCost;
	public bool canRecharge;
	public float rechargeRate;
	public float capacityRatio = 0;
	// Use this for initialization

	public bool isCharging;

	void Awake () {

		isActive=false;
		capacityLevel=abilityCapacity;
		isCharging=false;
	}

	void Update(){
		if(wControl.energyLevel<=0){
			if(isActive){
				deactivateModule ();
			}
		}

		if(isActive && !isCharging){
			if(energyCost>0){wControl.energyLevel -= energyCost*Time.deltaTime;}
			if(abilityCapacity>0){
				capacityLevel -= capacityCost * Time.deltaTime;
			
				if(capacityLevel<=0){
					isCharging = true;
					deactivateModule ();
				}

			}

			activatedUpdate();
		}
		else{
			if(abilityCapacity>0){
				capacityLevel=Mathf.Clamp (capacityLevel+rechargeRate*Time.deltaTime,0,abilityCapacity);
				if(capacityLevel == abilityCapacity){isCharging = false;}
			}
		}

		if(abilityCapacity>0)
			capacityRatio = capacityLevel/abilityCapacity;
	}

	public virtual void activateModule(){}

	public virtual void deactivateModule(){}

	public virtual void activatedUpdate(){}
}
