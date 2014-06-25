using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechAIHandler : MonoBehaviour {

	const float MAX_AIM_DISTANCE = 1000f;
	TerrainData levelTerrain;


	//control items
	MechController mControl;
	WeaponController wControl;
	CharacterController cControl;
	public GameObject MechType;

	//AI control
	bool hasMoveAITicked;
	public float minMoveTickTime=1f;
	public float maxMoveTickTime=3f;
	float nextMoveTick;

	bool hasCombatAITicked;
	public float minCombatTickTime=1f;
	public float maxCombatTickTime=3f;
	float nextCombatTick;

	public float targetingTimeoutPeriod = 10f;
	float targetingTimeout=0;

	//movement and steering variables
	Vector3 targetLocation;
	public float arrivalRadius =20f;

	public float relativeAngle;
	float absoluteAngle;
	float distanceToTarget;
	public float collisionScanDistance=100f;

	Vector3 steerLocation;
	int collMask; //Mask for general AI visibility casts. Initialised in Star().
	int terrainMask=(1<<12); //set up bitmask for terrain scan only



	//Weapons Targeting and combat
	public float minTargetAttackRadius=10f;
	public float maxTargetAttackRadius=500f;
	MechController myTarget;
	Ray aimRay;
	RaycastHit targetHit;
	public float defaultAggression = 0.8f;
	public float aggression=0.8f;
	GlobalTargetList targetList;

	Vector3 aimDriftTarget;
	Vector3 aimDrift;
	Vector3 targetVector;
	public float myAccuracyDrift=2f;

	bool hit;

	void Start () {
				
//		GameObject newMech=(GameObject)Instantiate (MechType,transform.position,transform.rotation);
//		newMech.transform.parent=transform;
		
		collMask = (1<<9)|(1<<13)|(1<<15); //set up mask to ignore HUD, weapon, and aimcast-permeable collision layers.
		collMask =~collMask;
		
		mControl=GetComponent<MechController>();
		wControl=GetComponent<WeaponController>();
		cControl=GetComponent<CharacterController>();
		
		targetLocation = transform.position;
		levelTerrain= GameObject.Find ("Terrain").GetComponent<Terrain>().terrainData;

		mControl.cockpit.gameObject.SetActive (false);

		GlobalTargetList.targetList.AddMech(mControl);

		populateMinMaxRanges();
		AITick ();
	}
	
	// Update is called once per frame
	void Update () {

		if(mControl!=null){
			if(!mControl.isDead){		

				AITick ();
				ScanForTargets();
				CalculateAim();
				CalculateShootyBang();
				DetermineWalkTarget();
				AvoidObstacles();
				CalculateSteering();

			}
		}
		
	}

	void populateMinMaxRanges()
	{
		if (wControl == null)
			return;

		if (wControl.weaponList == null)
			return;

		if (minTargetAttackRadius == null)
		{
			Debug.Log("minTargetAttackRadius is null");
			return;
		}

		for(int i=0; i < wControl.weaponList.Length; i++)
		{
			if (wControl.weaponList[i] == null)
			{
				Debug.Log("wControl.weaponList[i] is null");
				continue;
			}

			if (wControl.weaponList[i].weaponSafetyRange == null)
			{
				Debug.Log("wControl.weaponList[i].weaponSafetyRange is null");
				continue;
			}

			if(minTargetAttackRadius > wControl.weaponList[i].weaponSafetyRange)
				{
					minTargetAttackRadius = wControl.weaponList[i].weaponSafetyRange;
				}

			if(maxTargetAttackRadius>wControl.weaponList[i].weaponRange)
			{
				minTargetAttackRadius = wControl.weaponList[i].weaponRange;
			}
		}
	}

	void DetermineWalkTarget(){

		if(hasMoveAITicked){
			bool recalc=false;

			distanceToTarget = Vector2.Distance (new Vector2(targetLocation.x,targetLocation.z),new Vector2(mControl.transform.position.x,mControl.transform.position.z));
			//Debug.Log ("I'm this far from target: "+distanceToTarget.ToString ());
			if(distanceToTarget<=arrivalRadius){
				//Debug.Log ("At destination!");
				recalc=true;
			}

			if(myTarget==null){

				if(recalc){
					
					//Debug.Log ("Recalculating random target location");
					targetLocation= new Vector3(Random.Range(0,levelTerrain.size.x),0,Random.Range (0,levelTerrain.size.z));
					//Debug.Log ("targetLocation is "+targetLocation.ToString () +" out of size "+levelTerrain.size.ToString ());
				}
					
			}
			else{ //in combat

				//Debug.Log ("Recalculating position relative to attack target");
				targetLocation= myTarget.transform.position + (myTarget.transform.rotation*Quaternion.AngleAxis(Random.Range(-90,90),Vector3.up)*new Vector3(0,0,minTargetAttackRadius+(maxTargetAttackRadius-minTargetAttackRadius)*0.5f*(1-aggression)));
					
			}
			
		}
		
	}

	void AvoidObstacles(){

		if(hasMoveAITicked){

			if((cControl.collisionFlags & CollisionFlags.Sides)!= 0){
				mControl.setThrottle (-0.25f);
			}

			bool isBlocked=false;

			RaycastHit obstacle;
			isBlocked = Physics.Raycast (mControl.AIAimPoint.position,mControl.AIAimPoint.forward,out obstacle,100f,collMask);

			if(isBlocked){
				//Debug.Log ("There's a thing in my way");


				if(!obstacle.transform.CompareTag("Terrain")){
					//Debug.Log ("Bouncing.");
					Vector3 bouncevector=(obstacle.point-mControl.AIAimPoint.position);
					float bouncedistance=bouncevector.magnitude;
					bouncevector.Normalize();
					bouncedistance=Mathf.Clamp (bouncedistance,8f,50f);
					steerLocation=obstacle.point+(Vector3.Reflect (bouncevector,obstacle.normal)*bouncedistance);


					float newTick=Vector3.Distance (steerLocation,mControl.transform.position)/cControl.velocity.magnitude;
					nextMoveTick = Time.time+(newTick*0.9f);
					//Debug.Log ("Extending tick by "+newTick.ToString ());
				}
			}
			else{
				steerLocation=targetLocation;
			}
		}

	}

	void CalculateSteering(){
					
		Quaternion relativeRot = Quaternion.FromToRotation (mControl.transform.rotation*Vector3.forward,steerLocation-mControl.transform.position);
					
					relativeAngle=relativeRot.eulerAngles.y;


					absoluteAngle=Quaternion.Angle(mControl.transform.rotation,mControl.transform.rotation*relativeRot);
					
					if(myTarget==null)
					{
						if((absoluteAngle>5f)&&(distanceToTarget<10f))
						{
							
								mControl.setThrottle (0.1f);
							
						}
						else{mControl.setThrottle (0.8f);}
					}
					else{
						mControl.setThrottle(0.5f);
					}

					if(relativeAngle<175f){
						mControl.rotateMech (1);
					}
					else if(relativeAngle>185f){
						mControl.rotateMech(-1);
					}
					

								
	}
	

	void ScanForTargets(){
		if((hasCombatAITicked)||((myTarget==null)&&(mControl.hasTakenDamage))){
			//Debug.Log ("Scanning tick");
			bool findtarget=false;

			if(myTarget!=null){
				//Debug.Log ("I already have a target");

				if(myTarget.isDead)
					findtarget = true;

				if(Physics.Raycast (mControl.AIAimPoint.position,targetVector,targetVector.magnitude,terrainMask)){
					//Debug.Log ("My view of it is blocked");

					if(targetingTimeout==0){
						//Debug.Log ("Setting obstruction timeout");
						targetingTimeout=Time.time+targetingTimeoutPeriod;
					}
					else{

						if(targetingTimeout<=Time.time){
							//Debug.Log ("View obstruction means target has timed out");
							targetingTimeout=0;
							findtarget=true;
						}
					}
				}
				else{
					//Debug.Log ("I can see my target. Resetting timeout.");
					targetingTimeout=0; 
				}
			}
			else{
			
				findtarget=true;
			}

			if(findtarget){
				myTarget=null;
				//Debug.Log ("Scanning for target.");
				myTarget=GlobalTargetList.targetList.GetClosestTarget (mControl.AIAimPoint); 

				//if(myTarget!=null){Debug.Log ("Got a target!");}

			}

		}
	}

	
	void CalculateAim(){

		if(hasCombatAITicked){
			aimDriftTarget=new Vector3(Random.Range (-myAccuracyDrift,myAccuracyDrift),Random.Range (-myAccuracyDrift,myAccuracyDrift),Random.Range (-myAccuracyDrift,myAccuracyDrift));

		}

		aimDrift=Vector3.Slerp (aimDrift,aimDriftTarget,Time.deltaTime*0.5f);


		float relativeTargetAngle;

		if(myTarget!=null){
			targetVector = myTarget.AIAimPoint.position-mControl.AIAimPoint.position;
			Quaternion relativeTarget = Quaternion.FromToRotation (mControl.transform.rotation*Vector3.forward,targetVector);
			relativeTargetAngle=relativeTarget.eulerAngles.y;
		}
		else{
			targetVector=mControl.transform.forward;
			relativeTargetAngle=0;
		}

		if(relativeTargetAngle<180){
			relativeTargetAngle+=mControl.torsoRotation;
			if(relativeTargetAngle<0){
				mControl.rotateTorso (1);
			}
			else{
				mControl.rotateTorso (-1);
			}
		}
		else if(relativeTargetAngle>180){
			relativeTargetAngle+=mControl.torsoRotation;
			if(relativeTargetAngle>360){
				mControl.rotateTorso (-1);
			}
			else{
				mControl.rotateTorso (1);
			}
		}

		RaycastHit targetHit;

		hit=Physics.Raycast(mControl.AIAimPoint.position,targetVector,out targetHit,MAX_AIM_DISTANCE,collMask);
		
		if(hit){
			wControl.aimPoint=targetHit.point+aimDrift;
		}
		else{
			wControl.aimPoint=aimRay.GetPoint (MAX_AIM_DISTANCE);
		}
		
	}



	void CalculateShootyBang(){

		if(myTarget!=null){
			if(hasCombatAITicked){
				wControl.StopAllWeapons ();
				if(wControl.heatRatio<0.8f){
					if((wControl.inFireCone)&&(hit)){
						float targetdistance = targetVector.magnitude;
						if(targetdistance<=400f){
							if(!Physics.Raycast (mControl.AIAimPoint.position,targetVector,targetdistance,terrainMask)){
								for(int i =0; i<wControl.weaponList.Length;i++)
								{
									if(wControl.weaponList[i]!=null){
										if((targetdistance<wControl.weaponList[i].weaponRange) && (targetdistance>wControl.weaponList[i].weaponSafetyRange))
											wControl.weaponList[i].isActive = true;
										else
											wControl.weaponList[i].isActive = false;
									}
								}

								wControl.SetWeaponTriggers (true,0);
								wControl.SetWeaponTriggers (true,1);
							}
						
						}
					}
				}
			}
		}
		else{wControl.StopAllWeapons ();}

	}

	void AITick(){
		hasMoveAITicked=false;
		hasCombatAITicked=false;


			if(nextCombatTick<=Time.time){
				
				//Debug.Log ("Tick!");
				nextCombatTick = Time.time+Random.Range (minCombatTickTime,maxCombatTickTime);
				hasCombatAITicked=true;
				
				

			}

		if(nextMoveTick<=Time.time){
			//Debug.Log ("Movement tick");
			nextMoveTick = Time.time+Random.Range (minMoveTickTime,maxMoveTickTime);
			hasMoveAITicked=true;

		}

	}

	void OnDrawGizmos(){
		if(mControl!=null){
		Gizmos.color=Color.green;
		Gizmos.DrawLine (mControl.transform.position,steerLocation);
		Gizmos.DrawRay (mControl.transform.position,mControl.transform.forward);
		
		if(wControl.inFireCone)
			Gizmos.color=Color.cyan;
		else
			Gizmos.color=Color.blue;
		Gizmos.DrawLine (mControl.AIAimPoint.position,wControl.aimPoint);
		
		
		if(myTarget!=null){
		Gizmos.color=Color.red;
		Gizmos.DrawLine (mControl.transform.position,myTarget.transform.position);
		Gizmos.color=Color.yellow;
		Gizmos.DrawRay (mControl.AIAimPoint.position,targetVector);

		}
		Gizmos.color=Color.white;
		}

	}


}
