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
	public float minMoveTickTime=2f;
	public float maxMoveTickTime=5f;
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

	float relativeAngle;
	float absoluteAngle;
	float distanceToTarget;
	public float collisionScanDistance=100f;

	Vector3 steerLocation;
	int collMask; //Mask for general AI visibility casts. Initialised in Awake().
	int terrainMask=(1<<12); //set up bitmask for terrain scan only



	//Weapons Targeting an combat
	public float minTargetAttackRadius=25;
	public float maxTargetAttackRadius=75;
	MechController myTarget;
	Ray aimRay;
	RaycastHit targetHit;
	public float walkAggression=50f;
	GlobalTargetList targetList;

	Vector3 aimDriftTarget;
	Vector3 aimDrift;
	Vector3 targetVector;
	public float myAccuracyDrift=4f;

	bool hit;

	// Use this for initialization


	void Awake(){

	}
	
	
	void Start () {
				
//		GameObject newMech=(GameObject)Instantiate (MechType,transform.position,transform.rotation);
//		newMech.transform.parent=transform;
		
		collMask = (1<<9)|(1<<13); //set up mask to ignore HUD and weapon-permeable collision layers
		collMask =~collMask;
		
		mControl=GetComponent<MechController>();
		wControl=GetComponent<WeaponController>();
		cControl=GetComponent<CharacterController>();
		
		targetLocation = transform.position;
		levelTerrain= GameObject.Find ("Terrain").GetComponent<Terrain>().terrainData;

		GlobalTargetList.targetList.AddMech(mControl);

		AITick ();
	}
	
	// Update is called once per frame
	void Update () {

		if(mControl!=null){
			if(!mControl.isDead){		

			AITick ();

			DetermineWalkTarget();
			AvoidObstacles();
			CalculateSteering();
			ScanForTargets();
			CalculateAim();
			CalculateShootyBang();
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
			else{
				if((Random.Range (0,100)<=walkAggression)||(recalc)){
					//Debug.Log ("Recalculating position relative to attack target");
					targetLocation= myTarget.transform.position+ (Quaternion.AngleAxis(Random.Range(0,360),Vector3.up)*new Vector3(0,0,Random.Range (minTargetAttackRadius,maxTargetAttackRadius)));
				}
					
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
			isBlocked = Physics.Raycast (mControl.CockpitPosition.position,mControl.CockpitPosition.forward,out obstacle,100f,collMask);

			if(isBlocked){
				//Debug.Log ("There's a thing in my way");


				if(!obstacle.transform.CompareTag("Terrain")){
					//Debug.Log ("Bouncing.");
					Vector3 bouncevector=(obstacle.point-mControl.CockpitPosition.position);
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
					
					if(myTarget==null){
					if((absoluteAngle>5f)&&(distanceToTarget<10f)){
						
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

				if(Physics.Raycast (mControl.CockpitPosition.position,targetVector,targetVector.magnitude,terrainMask)){
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
				myTarget=GlobalTargetList.targetList.GetClosestTarget (mControl.CockpitPosition); 

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
			targetVector = myTarget.CockpitPosition.position-mControl.CockpitPosition.position;
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

		hit=Physics.Raycast(mControl.CockpitPosition.position,targetVector,out targetHit,MAX_AIM_DISTANCE,collMask);
		
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
							if(!Physics.Raycast (mControl.CockpitPosition.position,targetVector,targetdistance,terrainMask)){
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
		Gizmos.color=Color.yellow;
		Gizmos.DrawRay(mControl.torsoBone.position,mControl.torsoBone.forward);
		if(myTarget!=null){
		Gizmos.color=Color.red;
		Gizmos.DrawLine (mControl.transform.position,myTarget.transform.position);
		Gizmos.color=Color.yellow;
		Gizmos.DrawRay (mControl.CockpitPosition.position,targetVector);

		}
		Gizmos.color=Color.white;
		}

	}


}
