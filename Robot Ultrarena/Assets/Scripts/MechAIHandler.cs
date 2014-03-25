using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechAIHandler : MonoBehaviour {

	const float MAX_AIM_DISTANCE = 1000f;


	//control items
	MechController mControl;
	WeaponController wControl;
	CharacterController mCollider;
	public GameObject MechType;

	//AI control
	bool hasAITicked;
	public float minTickTime=1f;
	public float maxTickTime=2f;
	float nextTick;

	//movement and steering variables
	Vector3 targetLocation;
	public float arrivalRadius =4f;
	public Vector3 stageSize;
	public float relativeAngle;
	public float absoluteAngle;
	public float distanceToTarget;
	public float collisionScanDistance=50f;
	Vector3 avoidLocation;
	Vector3 steerLocation;
	int collMask;



	//Weapons Targeting an combat
	public float minTargetAttackRadius=10;
	public float maxTargetAttackRadius=50;
	Transform myTarget;
	Ray aimRay;
	RaycastHit targetHit;
	public float walkAggression=0.5f;
	GlobalTargetList targetList;

	Vector3 aimDriftTarget;
	Vector3 aimDrift;
	Vector3 targetVector;
	public float myAccuracyDrift=0;
	bool isGoodAim;
	bool hit;

	// Use this for initialization


	void Awake(){
		Transform targetContainer = GameObject.Find("TargetMechs").transform;
		this.transform.parent=targetContainer;
		targetList = targetContainer.GetComponent<GlobalTargetList>();

		GameObject newMech=(GameObject)Instantiate (MechType,transform.position,transform.rotation);
		newMech.transform.parent=this.transform;



		collMask = 1<<9;
		collMask =~collMask;

		AITick ();

	}
	
	
	void Start () {
		
		mControl=GetComponentInChildren<MechController>();
		wControl=GetComponentInChildren<WeaponController>();
		mCollider=GetComponentInChildren<CharacterController>();
		targetLocation = transform.position;
		isGoodAim=false;

	}
	
	// Update is called once per frame
	void Update () {
		if(mControl!=null){
			if(!mControl.isDead){		

			hasAITicked = AITick ();

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

		if(hasAITicked){

			bool recalc=false;
			if(distanceToTarget<=arrivalRadius){
				//Debug.Log ("At destination!");
				recalc=true;
			}

			if(myTarget==null){

				
				distanceToTarget = Vector3.Distance(targetLocation,mControl.transform.position);



				if(recalc){
					
					//Debug.Log ("Recalculating random target location");
					targetLocation= new Vector3(Random.Range(-stageSize.x,stageSize.x),0,Random.Range (-stageSize.z,stageSize.z));
				}
					
			}
			else{
				if((Random.Range (0,100)<=walkAggression)||(recalc)){
					//Debug.Log ("Recalculating position relative to attack target");
					targetLocation= myTarget.position+ (Quaternion.AngleAxis(Random.Range(0,360),Vector3.up)*new Vector3(0,0,Random.Range (minTargetAttackRadius,maxTargetAttackRadius)));
				}
					
			}
			
		}
		
	}

	void AvoidObstacles(){

		if ((mCollider.collisionFlags & CollisionFlags.Sides) != 0){

			mControl.setThrottle (0);
		}


		if(hasAITicked){
			bool isBlocked=false;

			RaycastHit obstacle;
			isBlocked = Physics.Raycast (mControl.transform.position+new Vector3(0,2,0),mControl.transform.forward,out obstacle,10f+(collisionScanDistance*mControl.throttleLevel),collMask);

			if(isBlocked){
				avoidLocation=obstacle.point+Vector3.Reflect ((obstacle.point-mControl.transform.position),obstacle.normal);
				steerLocation=avoidLocation;
				Debug.DrawLine(mControl.transform.position, obstacle.point, Color.red);
				Debug.DrawRay(obstacle.point, avoidLocation, Color.green);

				//Debug.Log ("Something's in my way "+obstacle.distance.ToString ()+"m ahead!");
				nextTick += obstacle.distance*0.1f;

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
		if(hasAITicked){
			myTarget=targetList.GetClosestTarget (mControl.transform);

			//if(myTarget!=null){Debug.Log ("Yay! I found a target!");}
		}
	}

	
	void CalculateAim(){

		if(hasAITicked){
			aimDriftTarget=new Vector3(Random.Range (-myAccuracyDrift,myAccuracyDrift),Random.Range (-myAccuracyDrift,myAccuracyDrift),Random.Range (-myAccuracyDrift,myAccuracyDrift));
		}

		aimDrift=Vector3.Slerp (aimDrift,aimDriftTarget,Time.deltaTime*0.1f);

		float relativeTargetAngle;

		if(myTarget!=null){
			targetVector = myTarget.position-mControl.transform.position;
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

		/*if(((relativeTargetAngle>=350)&&(relativeTargetAngle<360))||((relativeTargetAngle>0)&&(relativeTargetAngle<=10))){
			isGoodAim=true;
		}
		else{
			targetVector = mControl.torsoBone.forward;
			isGoodAim=false;
		}*/
		isGoodAim=true;


		RaycastHit targetHit;

		hit=Physics.Raycast(mControl.transform.position+new Vector3(0,4,0),targetVector,out targetHit,MAX_AIM_DISTANCE,collMask);
		
		if(hit){
			wControl.aimPoint=targetHit.point+aimDrift;
		}
		else{
			wControl.aimPoint=aimRay.GetPoint (MAX_AIM_DISTANCE);
		}
		
	}



	void CalculateShootyBang(){
		if(hasAITicked){
			wControl.StopAllWeapons ();
			if(myTarget!=null){
				if(wControl.heatRatio<0.8f){
				if((isGoodAim)&&(hit)){
					float targetdistance = targetVector.magnitude;
					if(targetdistance<=500f){
						if(!Physics.Raycast (mControl.transform.position,targetVector,targetdistance-0.5f)){
							wControl.SetWeaponTriggers (true,0);
							wControl.SetWeaponTriggers (true,1);
						}
						}
					}
				}
			}
		}
	}

	bool AITick(){
		if(nextTick<=Time.time){
			//Debug.Log ("Tick!");
			nextTick = Time.time+Random.Range (minTickTime,maxTickTime);
			return true;

		}
		else return false;
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
		Gizmos.DrawLine (mControl.transform.position,myTarget.position);
		}
		Gizmos.color=Color.white;
		}

	}


}
