using UnityEngine;
using System.Collections;

public class MechInputHandler : MonoBehaviour {

	public float deadZoneRange=0.15f;
	public float mouseStrength;

	public float cameraPitchMax = 15f;
	public float cameraPitch = 0;

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
	GameObject radarCam;

	float[] wepToggleButton = new float[2];

	public bool isShootyTarget{get;protected set;}

	public bool isTargeting{get;protected set;}

	bool deathRoutineDone=false;

	bool isCockpitView = false;

	// Use this for initialization
	void Awake(){

		playerController=this;
		deathCam = GameObject.Find ("DeathCamera");

		Screen.showCursor=false;

		Screen.lockCursor=true;

		radarCam = GameObject.Find ("RadarCamera");

	}


	void Start () {

		gameObject.layer=8;
		
		mControl=GetComponent<MechController>();
		wControl=GetComponent<WeaponController>();

		Camera.main.transform.parent=mControl.torsoBone;
		Camera.main.transform.localPosition=new Vector3(-8f,0,-20);
		Camera.main.transform.localEulerAngles=new Vector3(0f,0f,90f);

		radarCam.transform.position = new Vector3(transform.position.x,radarCam.transform.position.y,transform.position.z);
		radarCam.transform.parent=mControl.torsoBone;

		transform.FindChild("RadarBlip").renderer.material.color = Color.green;

		deathCam.GetComponent<DeathCameraScript>().playerMech=mControl.AIAimPoint;
		deathCam.SetActive(false);

		mControl.cockpit.gameObject.SetActive (isCockpitView);


		playerMask = 1<<8;
		weaponPermeableMask = 1<<13;
		playerMask = playerMask | weaponPermeableMask |(1<<15);
		playerMask =~playerMask;

		GlobalTargetList.targetList.AddMech (mControl);

		SetCameraPosition();

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
			MouseCameraPitch ();
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

		if(Input.GetKey(KeyCode.Escape))
			mControl.armourLevel = 0;

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

		if(Input.GetKeyUp (KeyCode.F)){

			wControl.toggleModule(false,0);
		}

		if(Input.GetKeyDown (KeyCode.F)){

			wControl.toggleModule(true,0);
		}

		if(Input.GetKeyUp (KeyCode.G)){

			wControl.toggleModule(false,1);
		}
		
		if(Input.GetKeyDown (KeyCode.G)){

			wControl.toggleModule(true,1);
		}

		if(Input.GetKeyUp (KeyCode.C)){
			
			isCockpitView = !isCockpitView;
			SetCameraPosition();
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

	void MouseCameraPitch(){
		
		float direction = Mathf.Clamp (Input.mousePosition.y-(Screen.height/2),-1,1);			
		mouseStrength=2*Mathf.Abs (((Input.mousePosition.y-(Screen.height/2))/Screen.height));
		mouseStrength=Mathf.Clamp01 (mouseStrength-deadZoneRange);

		cameraPitch = Mathf.Clamp (cameraPitch-mouseStrength*direction,-cameraPitchMax,cameraPitchMax);

		Camera.main.transform.localEulerAngles=new Vector3(0,cameraPitch,90);

	}

	void SetCameraPosition()
	{
		if(isCockpitView)
		{
			mControl.cockpit.gameObject.SetActive (true);
			Camera.main.transform.localPosition = mControl.cockpit.transform.localPosition;
		}
		else
		{
			mControl.cockpit.gameObject.SetActive (false);
			Camera.main.transform.localPosition = mControl.cockpit.localPosition + new Vector3(-4,0,-20);
		}
	}
	
}
