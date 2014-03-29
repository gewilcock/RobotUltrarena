using UnityEngine;
using System.Collections;

public class MechInputHandler : MonoBehaviour {

	public static MechInputHandler playerController{get;private set;}

	const float MAX_AIM_DISTANCE = 1000f;
	public float throttleAdjustRate=0.2f;
	public MechController mControl;
	public WeaponController wControl;
	public GameObject MechType;

	Ray aimRay;
	RaycastHit targetHit;
	int playerMask;
	int weaponPermeableMask;

	HUDTargetBracketScript currentBracket;
	public GameObject bracketObject;
	public GameObject HUDObject;
	GameObject deathCam;

	public bool isShootyTarget;

	bool deathRoutineDone=false;

	// Use this for initialization
	void Awake(){

		playerController=this;
		deathCam = GameObject.Find ("DeathCamera");
		Screen.showCursor=false;


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

		bracketObject=(GameObject)Resources.Load ("UITargetingBracket");

		deathCam.GetComponent<DeathCameraScript>().playerMech=mControl.CockpitPosition;
		deathCam.SetActive(false);


		playerMask = 1<<8;
		weaponPermeableMask = 1<<13;
		playerMask = playerMask | weaponPermeableMask;
		playerMask =~playerMask;

		GlobalTargetList.targetList.AddMech (mControl);

		currentBracket=null;
	}
	
	// Update is called once per frame
	void Update () {
		if(mControl!=null){
		if(!mControl.isDead){
			GetSteeringInput();
			MouseAim();
			GetWeaponInput();
		}
		else{
			if(!deathRoutineDone){
			if(currentBracket!=null){
				Destroy (currentBracket.gameObject);
				currentBracket=null;
			}
			
			
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

		mControl.setThrottle (mControl.throttleLevel + (Input.GetAxis("Throttle")*throttleAdjustRate*Time.deltaTime));



		mControl.rotateMech (Input.GetAxis ("Steering"));
		mControl.rotateTorso (Input.GetAxis ("TorsoTwist"));

		if(Input.GetButton ("AllStop")){mControl.allStop();}

	}

	void GetWeaponInput(){

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

		if(Input.GetButtonUp("ToggleWepGroup0")){
			wControl.ToggleActiveWeapons(0);
		}

		if(Input.GetButtonUp("ToggleWepGroup1")){
			wControl.ToggleActiveWeapons(1);
		}

		if(Input.GetButtonDown ("TargetUnderReticle")){

			bool spawnNew = false;
			RaycastHit targetingHit;
			bool hittarget=Physics.Raycast(aimRay, out targetingHit,MAX_AIM_DISTANCE,(playerMask));

			if(hittarget){
			Transform test = targetingHit.collider.transform;

			if(test.CompareTag ("DamageObject")){
			if(currentBracket!=null){
					Destroy (currentBracket.gameObject);
					currentBracket=null;
					spawnNew=true;
			}
			else{
				spawnNew=true;
			}

			if(spawnNew){
				
				GameObject weh=(GameObject)Instantiate(bracketObject,test.position,Quaternion.identity);
				currentBracket=weh.GetComponent<HUDTargetBracketScript>();
				currentBracket.myParent=test;
				currentBracket.playerMech=mControl.transform;
			}
			}
			}
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
	


}
