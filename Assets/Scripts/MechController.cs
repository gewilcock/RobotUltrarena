using UnityEngine;
using System.Collections;

public class MechController : MonoBehaviour {

	//Script controls all major variables and mech systems

	//Reference scripts
	private CharacterMotor mechMotor;
	private WeaponController mechWeapons;
	private Animation animations;

	//armour and armagel variables

	public bool isDead{get;protected set;}

	public float armourLevel = 100f;
	public float maxArmour = 100f;

	float armagelLevel;
	public float maxArmagelLevel = 50f;
	public float armagelUsageRate = 5f;

	//transforms and variables for torso rotation
	public Transform torsoBone;
	public float torsoRotationRate=20f;
	public float torsoAbsMaxRotation=90f;
	float torsoRotation=0f;

	//variables for mech movement
	public float mechTurnRate=10f;
	public float throttleAdjustRate=0.2f;
	float throttleLevel=0.5f;
	float maxThrottle=1f;
	float minThrottle=-0.25f;


	// Use this for initialization
	void Start () {
	mechMotor = GetComponent <CharacterMotor>();
	animations = GetComponentInChildren<Animation>();
	mechWeapons=GetComponentInChildren<WeaponController>();
	isDead=false;
	}
	
	// Update is called once per frame
	void Update () {

		mechMotor.inputMoveDirection = transform.rotation*Vector3.forward*throttleLevel;
		AssessDamage();
		ApplyAnimations();

	}


	public void rotateTorso(float direction){

		torsoRotation=Mathf.Clamp (torsoRotation+(direction*torsoRotationRate)*Time.deltaTime,-torsoAbsMaxRotation,torsoAbsMaxRotation);
		torsoBone.localEulerAngles = new Vector3(torsoRotation,270f,0f);
		
	}

	public void rotateMech(float direction){
		transform.Rotate (new Vector3(0f,mechTurnRate*Time.deltaTime*direction));
	}

	public void adjustThrottle(float direction){
		if(direction != 0f){
			throttleLevel=Mathf.Clamp(throttleLevel+(throttleAdjustRate*direction*Time.deltaTime),minThrottle,maxThrottle);
		}

			
	}

	public void allStop(){
		
		throttleLevel=0f;

	}

	public void ApplyAnimations(){
	
		if(!isDead){

			if((throttleLevel/maxThrottle)==0f){
			animations.CrossFade ("Stationary",0.5f);
			}
			else{
			if(!animations.IsPlaying("Walk")){animations.CrossFade ("Walk",2f);}
			animations["Walk"].speed = (throttleLevel/maxThrottle);
			}
		}
		else{

			animations.CrossFade ("Death",1.5f);

		}

	}

	public float GetThrottleLevel(){
		return throttleLevel;
	}

	public void TakeDamage(float damage){
		armourLevel-=damage;
	}

	void AssessDamage(){
		if(!isDead){
			if(armourLevel<=0f){
				isDead=true;
				mechWeapons.isDead=true;
				allStop ();
			}
		}
		else{
			if(!animations.isPlaying){
				Destroy (gameObject);
			}
		}
	}
}
