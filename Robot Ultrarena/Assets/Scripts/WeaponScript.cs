using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

	//References
	public WeaponController myController;
	public Transform muzzlePoint;
	public GameObject myProjectile;

	//4-character name for display
	public string weaponName;

	//Variables for determining fire state and for HUD to leech off of.
	public bool isActive{get;set;}
	public bool triggered{get;set;}
	public bool isOverheating{get;protected set;}
	public bool isInRange{get;protected set;}
	public float weaponRange;
	public bool canFire{get; protected set;}
	bool isFiring;

	//General refire variables
	public float refireTime;
	float nextFireTime;
	public float refireRatio;

	//Accuracy
	public float fireConeAngle;

	//Per-weapon heat variables
	float weaponHeat;
	public float maxHeat;
	public float heatPerShot;
	public float heatSinkRate;
	public float heatRatio{get;protected set;}

	//How many projectiles are to be spawned?
	public int projectilesPerShot; 

	//Weapon ammo levels
	public int ammoLevel;
	public int ammoMaxLevel;

	//Energy cost
	public float energyPerShot;

	// Use this for initialization
	void Awake () {
		muzzlePoint=transform.FindChild ("muzzlePoint");
		nextFireTime = Time.time;
		isActive=true;
	}
	
	// Update is called once per frame
	void Update () {

		canFire=CheckRefireVariables();		
		if(isActive){
			if(triggered && canFire){
				FireWeapon();
			}
		}

		ApplyHeat();

		refireRatio=(nextFireTime-Time.time)/refireTime;
		refireRatio=Mathf.Clamp (refireRatio,0,1);
	}

	private void FireWeapon(){

			for(int i = 0; i < projectilesPerShot; i++){

				Quaternion fireDirection = transform.rotation;
				if(fireConeAngle>0f){
				fireDirection.eulerAngles += new Vector3(Random.Range (-fireConeAngle,fireConeAngle),Random.Range (-fireConeAngle,fireConeAngle),0f);

				}
					
				GameObject pew = (GameObject)Instantiate (myProjectile,muzzlePoint.position,fireDirection);
				pew.transform.parent=muzzlePoint;
				
				
			}

		weaponHeat+=heatPerShot;
		myController.heatLevel+=heatPerShot;
		
		myController.energyLevel-=energyPerShot;
		
		if(ammoMaxLevel>0){
			ammoLevel--;
		}
		
		nextFireTime=Time.time+refireTime;

	}

	bool CheckRefireVariables(){

		if((!isOverheating)&&(!myController.isOverheating)){
				if(Time.time>=nextFireTime){
					if((ammoMaxLevel==0) || ((ammoMaxLevel>0) && (ammoLevel>0))){
						if((energyPerShot==0) || ((energyPerShot>0) && (myController.energyLevel>0))){
							return true;
						}
					}
				}

			
		}

		return false;

	}

	void ApplyHeat(){

		weaponHeat=Mathf.Clamp (weaponHeat-(heatSinkRate*Time.deltaTime),0,maxHeat*1.5f);

		if(myController.isOverheating){
			isOverheating=true;
		}
		else{
			if(weaponHeat>maxHeat){isOverheating=true;}

			if((isOverheating)&&(weaponHeat==0)){isOverheating=false;}
		}
		heatRatio=weaponHeat/maxHeat;
	}



}
