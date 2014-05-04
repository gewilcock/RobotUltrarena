using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

	//This class defines and controls individual weapon behaviours and states. It is triggered and activated by its mech's WeaponsController script.

	//Is the projectile attached to the muzzle transform (lasers, raybullets)?
	public bool isDiscreteProjectile;

	//Control References
	public WeaponController myController;
	Transform muzzlePoint;
	public GameObject myProjectile;

	//4-character name for display
	public string weaponName;

	//Variables for determining weapon state and for HUD to leech info off of.
	public bool isActive{get;set;}
	public bool triggered{get;set;}
	public bool isOverheating{get;protected set;}
	public float weaponRange;
	public bool canFire{get; protected set;}


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

	//How many projectiles are to be spawned per shot?
	public int projectilesPerShot; 

	//Weapon ammo levels
	public int ammoLevel;
	public int ammoMaxLevel;
	public int ammoPerShot = 1;

	//Energy cost
	public float energyPerShot;

	//Variables to determine whether the weapon requires a lock before, and control variables to handle this and pass to HUD/AI.
	public bool requiresLock;
	public float lockOnDuration;
	float lockOnTime;
	public float lockOnRatio{get;protected set;}
	public Transform lockTransform{get; protected set;}
	Transform lastLockTransform;

	Ray lockRay;
	int lockMask = ~((1<<9)|(1<<13)); //set up locking coll mask to ignore weapon-permeable and HUD meshes.

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
				if(requiresLock){
					if(lockTransform==null){
						lockOnTime = Time.time + lockOnDuration;
					}
					LockOnTargetAndFire();
				}
				else{
					FireWeapon();
				}
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

			//Attach the projectile to the muzzle transform (for lasers), otherwise it can go its own way.
			if(!isDiscreteProjectile){
				pew.transform.parent=muzzlePoint;
			}
			else{
			Physics.IgnoreCollision(pew.collider, myController.collider);
			}

			if(requiresLock){
				pew.GetComponent<MissileSpawnerScript>().lockedTarget = lockTransform;
			}
				
			}

		weaponHeat+=heatPerShot;
		myController.heatLevel+=heatPerShot;
		
		myController.energyLevel-=energyPerShot;
		
		if(ammoMaxLevel>0){
			ammoLevel-=ammoPerShot;
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


	void LockOnTargetAndFire(){

		lockRay = new Ray(muzzlePoint.position,myController.aimPoint-muzzlePoint.position);
		RaycastHit lockTarget;
		bool hit = Physics.Raycast(lockRay,out lockTarget,5000f,lockMask);
		
		if(hit){
			if(lockTarget.transform.CompareTag("DamageObject")){
				lockTransform = lockTarget.transform;
			}
			else{
				lockTransform = null;
			}
		}
		else{
			lockTransform = null;

		}

		if((lastLockTransform != null) && (lockTransform == lastLockTransform)){
			if(lockOnTime <= Time.time){
				Debug.Log ("FIRE MISSILES!");
				FireWeapon ();
				lockTransform=null;
			}
		}
		else{
			lockOnTime=Time.time+lockOnDuration;

		}

		lastLockTransform = lockTransform;
		lockOnRatio = ((lockOnTime-Time.time)/lockOnDuration);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		Gizmos.DrawRay (lockRay);
		if(lockTransform!=null){
		Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(lockTransform.position,10f);
		}

	}
}
