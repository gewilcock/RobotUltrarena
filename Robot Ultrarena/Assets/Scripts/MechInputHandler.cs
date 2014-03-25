using UnityEngine;
using System.Collections;

public class MechInputHandler : MonoBehaviour {

	const float MAX_AIM_DISTANCE = 1000f;
	public float throttleAdjustRate=0.2f;
	public MechController mControl;
	public WeaponController wControl;
	public GameObject MechType;

	Ray aimRay;
	RaycastHit targetHit;
	int playerMask;

	HUDTargetBracketScript currentBracket;
	public GameObject bracketObject;

	GameObject deathCam;

	// Use this for initialization
	void Awake(){
		GameObject newMech=(GameObject)Instantiate (MechType,transform.position,transform.rotation);
		newMech.transform.parent=this.transform;
		newMech.layer=8;

		Camera.main.transform.parent=newMech.transform.GetComponentInChildren<MechController>().torsoBone;

		GameObject.Find ("RadarCamera").transform.parent=newMech.transform;

		deathCam = GameObject.Find ("DeathCamera");
		deathCam.SetActive(false);

		playerMask = 1<<8;
		playerMask =~playerMask;
	}


	void Start () {

		mControl=GetComponentInChildren<MechController>();
		wControl=GetComponentInChildren<WeaponController>();
		GameObject.Find ("HUD").transform.BroadcastMessage ("Start");

		currentBracket=null;
	}
	
	// Update is called once per frame
	void Update () {
		if(!mControl.isDead){
			GetSteeringInput();
			MouseAim();
			GetWeaponInput();
		}
		else{

			Destroy (currentBracket.gameObject);
			currentBracket=null;

			deathCam.transform.position=Camera.main.transform.position;
			deathCam.transform.LookAt(mControl.transform.position);
			Camera.main.gameObject.SetActive (false);
			deathCam.SetActive(true);
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
			Ray targetingRay=Camera.main.ScreenPointToRay(Input.mousePosition);
			bool hit=Physics.Raycast(aimRay, out targetingHit,MAX_AIM_DISTANCE,playerMask);

			if(hit){
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

		bool hit;

		aimRay=Camera.main.ScreenPointToRay(Input.mousePosition);
		hit=Physics.Raycast(aimRay, out targetHit,MAX_AIM_DISTANCE,playerMask);

		if(hit){
			wControl.aimPoint=targetHit.point;
		}
		else{
			wControl.aimPoint=aimRay.GetPoint (MAX_AIM_DISTANCE);
		}

	}
	


}
