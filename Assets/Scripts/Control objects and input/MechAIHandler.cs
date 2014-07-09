using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
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
	public float minMoveTickTime=5f;
	public float maxMoveTickTime=10f;
	float nextMoveTick;

	bool hasCombatAITicked;
	public float minCombatTickTime=1f;
	public float maxCombatTickTime=3f;
	float nextCombatTick;

	public float targetingTimeoutPeriod = 10f;
	float targetingTimeout=0;

	//movement and steering variables
	Seeker mySeeker;
	public Path myPath;
	bool recalcWalkTarget = true;
	private int currentWaypoint = 0;
	Vector3 targetLocation;
	public float arrivalRadius =20f;

	public float relativeAngle;
	public float absoluteAngle;
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

		mySeeker = (Seeker)gameObject.AddComponent ("Seeker");
		mySeeker.startEndModifier.useRaycasting = true;
		/*mySeeker.startEndModifier.useGraphRaycasting = true;*/
		mySeeker.startEndModifier.exactEndPoint=StartEndModifier.Exactness.Interpolate;
		mySeeker.startEndModifier.exactStartPoint=StartEndModifier.Exactness.Original;

		gameObject.AddComponent ("FunnelModifier");

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
				//AvoidObstacles();
				CalculateSteering();

			}
		}
		
	}

	void populateMinMaxRanges()
	{
		for(int i=0; i<wControl.weaponList.Length; i++)
		{
			if(minTargetAttackRadius>wControl.weaponList[i].weaponSafetyRange)
			{
				minTargetAttackRadius = wControl.weaponList[i].weaponSafetyRange;
			}

			if(maxTargetAttackRadius>wControl.weaponList[i].weaponRange)
			{
				minTargetAttackRadius = wControl.weaponList[i].weaponRange;
			}
		}
	}

	public void DetermineWalkTarget(){			
		if(hasMoveAITicked)
		{
			if(myTarget==null){
				if(recalcWalkTarget)		
					targetLocation= new Vector3(Random.Range(0,levelTerrain.size.x),0,Random.Range (0,levelTerrain.size.z));					
					
			}
			else{ //in combat
				if(Random.Range (0,1)<=aggression)
					targetLocation= myTarget.transform.position+myTarget.transform.rotation*Quaternion.AngleAxis(180,Vector3.up)*(Vector3.forward*maxTargetAttackRadius*(1-aggression));

			}

			mySeeker.StartPath (transform.position,targetLocation, OnPathComplete);
			recalcWalkTarget = false;
		}

		
	}

	public void OnPathComplete (Path p) {
		Debug.Log ("Path returned. Error: "+p.error);
		if (!p.error) {
			myPath = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
			
		}
	}

	public virtual void targetReached()
	{
		recalcWalkTarget = true;
		hasMoveAITicked = true;
	}

	void FixedUpdate()
	{



				if (myPath == null) {
					//We have no path to move after yet
					return;
				}
				if (currentWaypoint >= myPath.vectorPath.Count) {
					
					targetReached ();
					return;
					
				}
				
				//set steering location to next waypoint
				steerLocation = myPath.vectorPath[currentWaypoint];
				
				//Check if we are close enough to the next waypoint
				//If we are, proceed to follow the next waypoint
				if (Vector3.Distance (transform.position,myPath.vectorPath[currentWaypoint]) <= arrivalRadius) {
					currentWaypoint++;
					return;
				}



	}

	void CalculateSteering(){
					
		Quaternion relativeRot = Quaternion.FromToRotation (mControl.transform.rotation*Vector3.forward,steerLocation-mControl.transform.position);
					
					relativeAngle=relativeRot.eulerAngles.y;
			
					

					absoluteAngle=Quaternion.Angle(mControl.transform.rotation,mControl.transform.rotation*relativeRot);
					
					if(myTarget==null)
					{
						if((absoluteAngle>30f))
						{
							
								mControl.setThrottle (0.1f);
							
						}
						else{mControl.setThrottle (0.8f);}
					}
					else{
						mControl.setThrottle(0.5f);
					}

					if(relativeAngle<180f){
						mControl.rotateMech (1);
					}
					else if(relativeAngle>180f){
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
