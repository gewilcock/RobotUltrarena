﻿using UnityEngine;
using System.Collections;

public class MechInputHandler : MonoBehaviour {

	public float deadZoneRange=0.15f;
	public float mouseStrength;

	public bool absThrottle=false;

	public static MechInputHandler playerController{get;private set;}

	public const float MAX_AIM_DISTANCE = 1000f;
	public float throttleAdjustRate=0.75f;
	public MechController mControl;
	public WeaponController wControl;
	public GameObject MechType;

	Ray aimRay;
	RaycastHit targetHit;
	int playerMask;
	int weaponPermeableMask;

	public GameObject HUDObject;
	GameObject deathCam;

	float[] wepToggleButton = new float[2];

	public bool isShootyTarget{get;protected set;}

	public bool isTargeting{get;protected set;}

	bool deathRoutineDone=false;

	// Use this for initialization
	void Awake(){

		playerController=this;
		deathCam = GameObject.Find ("DeathCamera");
		Screen.showCursor=false;

		Screen.lockCursor=true;

	}


	void Start () {

//		GameObject newMech=(GameObject)Instantiate (MechType,transform.position,transform.rotation);
//		newMech.transform.parent=this.transform;
		gameObject.layer=8;
		
		mControl=GetComponent<MechController>();
		wControl=GetComponent<WeaponController>();

		Camera.main.transform.parent=mControl.torsoBone;
		Camera.main.transform.localPosition=new Vector3(-8f,0,-20);
		Camera.main.transform.localEulerAngles=new Vector3(0f,0f,90f);

		GameObject.Find ("RadarCamera").transform.parent=mControl.transform;

		deathCam.GetComponent<DeathCameraScript>().playerMech=mControl.CockpitPosition;
		deathCam.SetActive(false);


		playerMask = 1<<8;
		weaponPermeableMask = 1<<13;
		playerMask = playerMask | weaponPermeableMask |(1<<15);
		playerMask =~playerMask;

		GlobalTargetList.targetList.AddMech (mControl);



		wepToggleButton[0]=-1;
		wepToggleButton[1]=-1;

		Screen.lockCursor=false;
	}
	
	// Update is called once per frame
	void Update () {
		if(mControl!=null){
		if(!mControl.isDead){
			GetSteeringInput();
			MouseTurretAdjustment ();
			MouseAim();
			
			GetWeaponInput();
		}
		else{
			if(!deathRoutineDone){
						
			
			deathCam.transform.position=Camera.main.transform.position;
			
			foreach(Camera c in Camera.allCameras){c.gameObject.SetActive (false);}
			
			deathCam.SetActive(true);
			deathCam.transform.LookAt(mControl.transform.position);
			deathRoutineDone=true;
			}
		}
		}

	}

	void GetSteeringInput(){

		if(!absThrottle){
		mControl.setThrottle (mControl.throttleLevel + (Input.GetAxis("Throttle")*throttleAdjustRate*Time.deltaTime));
		}
		else{mControl.setThrottle (Input.GetAxis ("Throttle"));}



		mControl.rotateMech (Input.GetAxis ("Steering"));
		//mControl.rotateTorso (Input.GetAxis ("TorsoTwist"));

		if(Input.GetButton ("AllStop")){mControl.allStop();}

	}

	void GetWeaponInput(){
		if(Input.GetButtonDown ("HeavyFire1")){
			wControl.setHeavyWeaponTrigger (true,0);
		}

		if(Input.GetButtonUp ("HeavyFire1")){
			wControl.setHeavyWeaponTrigger (false,0);
		}

		if(Input.GetButtonDown ("HeavyFire2")){
			wControl.setHeavyWeaponTrigger (true,1);
		}
		
		if(Input.GetButtonUp ("HeavyFire2")){
			wControl.setHeavyWeaponTrigger (false,1);
		}

		if(Input.GetButton ("Fire1")){
			wControl.SetWeaponTriggers (true,0);
		}
		else{
			wControl.SetWeaponTriggers (false,0);
		}

		if(Input.GetButton ("Fire2")){
			wControl.SetWeaponTriggers (true,1);
		}
		else{
			wControl.SetWeaponTriggers (false,1);
		}

		if(Input.GetButtonDown("ToggleWepGroup0")){

			wepToggleButton[0]=Time.time+0.3f;

		}
		
		if(Input.GetButtonDown("ToggleWepGroup1")){
			wepToggleButton[1]=Time.time+0.3f;

		}

		if(Input.GetButton("ToggleWepGroup0")){
			if(wepToggleButton[0]<=Time.time){
				wControl.SetGroupFire(0);
			}
		}

		if(Input.GetButton("ToggleWepGroup1")){
			if(wepToggleButton[1]<=Time.time){
				wControl.SetGroupFire(1);
			}
		}


		if(Input.GetButtonUp("ToggleWepGroup0")){
			if(wepToggleButton[0]>Time.time){
				wControl.ToggleActiveWeapons(0);
			}
		}

		if(Input.GetButtonUp("ToggleWepGroup1")){
			if(wepToggleButton[1]>Time.time){
				wControl.ToggleActiveWeapons(1);
			}
		}

		if(Input.GetButtonDown ("TargetUnderReticle")){
			isTargeting = true;
		}
		else{isTargeting=false;}

		if(Input.GetKeyUp (KeyCode.Space)){
			Debug.Log ("Space up");
			wControl.toggleModule(false,0);
		}

		if(Input.GetKeyDown (KeyCode.Space)){
			Debug.Log ("Space down");
			wControl.toggleModule(true,0);
		}

	}

	void MouseAim(){

		aimRay=Camera.main.ScreenPointToRay(Input.mousePosition);
		bool hit=Physics.Raycast(aimRay, out targetHit,MAX_AIM_DISTANCE,(playerMask));
		isShootyTarget=false;

		if(hit){
			wControl.aimPoint=targetHit.point;
			if(targetHit.transform.CompareTag ("DamageObject")){isShootyTarget=true;}
		}
		else{
			wControl.aimPoint=aimRay.GetPoint (MAX_AIM_DISTANCE);
		}

	}

	void MouseTurretAdjustment(){

		Vector3 targetVector = wControl.aimPoint-transform.position;
		float relativeAngle = Quaternion.FromToRotation (mControl.transform.rotation*Vector3.forward,targetVector).eulerAngles.y;


		mouseStrength=2*Mathf.Abs (((Input.mousePosition.x-(Screen.width/2))/Screen.height));
		mouseStrength=Mathf.Clamp01 (mouseStrength-deadZoneRange);

		if(relativeAngle<180){
			relativeAngle+=mControl.torsoRotation;
			if(relativeAngle<0){
				mControl.rotateTorso (mouseStrength);
			}
			else if(relativeAngle>0){
				mControl.rotateTorso (-mouseStrength);
			}
		}
		else if(relativeAngle>180){
			relativeAngle+=mControl.torsoRotation;
			if(relativeAngle>360){
				mControl.rotateTorso (-mouseStrength);
			}
			else if(relativeAngle<360){
				mControl.rotateTorso (mouseStrength);
			}
		}
	}
}
