﻿using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {



	//heat variables
	public MechController myController;
	public float heatLevelMax = 100f;
	public float heatSinkRate = 10f;
	public float heatLevel{get; set;}
	public float heatRatio{get;protected set;}

	public bool isOverheating{get;protected set;}
	public bool isDead{get;set;}

	//Variables for controlling weapons
	public WeaponScript[] weaponList = new WeaponScript[4];
	public int weaponsPerGroup =2;
	int[] groupCycle = new int[2];


	//Energy cache variables
	public float energyLevel;
	public float energyRechargeRate = 5f;
	public float maxEnergyLevel = 100f;
	public float energyRatio{get;protected set;}

	//Variables for aim calculation
	public Vector3 aimPoint;
	public Transform[] aimPivots = new Transform[2];
	public float yawLimit=45f;
	public float pitchLimit=30f;

	public bool inFireCone;

	// Use this for initialization
	void Start () {
		isDead=false;
		energyLevel=maxEnergyLevel;
		isOverheating=false;

		for(int i=0; i<weaponList.Length;i++){
			WeaponScript w = weaponList[i];
			if(w!=null){
				w.myController=this;	
			}
		}

		groupCycle[0]=weaponsPerGroup;
		groupCycle[1]=weaponsPerGroup;

		aimPoint = transform.position+transform.forward*500f;
	}

	void Update(){
		if(!isDead){
			ApplyHeat();
			ApplyEnergy();
		}

		
	}
	
	void LateUpdate () {

		if(!isDead){
			inFireCone=true;

			for(int i=0; i<aimPivots.Length; i++){

				Vector3 targetDir = aimPoint - aimPivots[i].position;										 

				Quaternion testRotation=Quaternion.LookRotation (targetDir);

				Quaternion baseRotation=transform.rotation*Quaternion.Euler (0f,-aimPivots[i].parent.localRotation.eulerAngles.x,0);


				Quaternion relativeRot = testRotation *Quaternion.Inverse (baseRotation);

				float dx;
				float dy;

				if(relativeRot.eulerAngles.y<180f){
					dy = Mathf.Clamp (relativeRot.eulerAngles.y,0,yawLimit);
				}
				else {
					dy = Mathf.Clamp (relativeRot.eulerAngles.y,360-yawLimit,360f);
				}

				if(relativeRot.eulerAngles.x<180f){
					dx = Mathf.Clamp (relativeRot.eulerAngles.x,0,pitchLimit);
				}
				else {
					dx = Mathf.Clamp (relativeRot.eulerAngles.x,360-pitchLimit,360f);
				}


				if((dy != relativeRot.eulerAngles.y)||(dx != relativeRot.eulerAngles.x)){inFireCone=false;}

				aimPivots[i].rotation = Quaternion.Euler (new Vector3(dx,dy,0))*baseRotation;


			}
		}
		
	}

	public void SetWeaponTriggers(bool triggerstate,int weaponGroup){

		int fireGroupBase=weaponGroup*weaponsPerGroup;

		for(int i=fireGroupBase; i<fireGroupBase+weaponsPerGroup;i++){
			if(weaponList[i]!=null){
				weaponList[i].triggered=triggerstate;
			}
		}

	}

	public void ToggleActiveWeapons(int weaponGroup){
		
		groupCycle[weaponGroup]++;
		if(groupCycle[weaponGroup]>weaponsPerGroup){groupCycle[weaponGroup]=0;}

		int fireGroupBase=weaponGroup*weaponsPerGroup;

		for(int i=fireGroupBase; i<fireGroupBase+weaponsPerGroup;i++){

			WeaponScript thisWeapon=weaponList[i];

			if(thisWeapon!=null){
				if(groupCycle[weaponGroup]<weaponsPerGroup){
					if((i-fireGroupBase)==groupCycle[weaponGroup]){
						thisWeapon.isActive=true;
					}
					else{
						thisWeapon.isActive=false;
					}
				}
				else{
					thisWeapon.isActive=true;
				}
			}
		
		}
		
	}

	void ApplyHeat(){

		heatLevel=Mathf.Clamp (heatLevel-(heatSinkRate*Time.deltaTime),0,heatLevelMax*1.5f);

		if(heatLevel>heatLevelMax){isOverheating=true;}
		
		if((isOverheating)&&(heatLevel==0)){isOverheating=false;}

		heatRatio=Mathf.Clamp (heatLevel/heatLevelMax,0,1f);
	}

	void ApplyEnergy(){
			if(!isOverheating){
				energyLevel=Mathf.Clamp (energyLevel+energyRechargeRate*Time.deltaTime,-maxEnergyLevel,maxEnergyLevel);
			}

		energyRatio=Mathf.Clamp (energyLevel/maxEnergyLevel,0f,1f);
	}

	public void StopAllWeapons(){

		for(int i=0; i<weaponList.Length;i++){
			if(weaponList[i]!=null){
				weaponList[i].triggered=false;
			}
		}
		
	}

}

