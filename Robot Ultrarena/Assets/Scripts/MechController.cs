using UnityEngine;
using System.Collections;

public class MechController : MonoBehaviour {

	//Script controls all major variables and mech systems

	public GameObject deathExplosion;
	//Reference scripts
	private CharacterMotor mechMotor;
	private WeaponController mechWeapons;
	private Animation animations;

	//armour and armagel variables

	public bool isDead{get;protected set;}

	public float armourLevel = 100f;
	public float maxArmour = 100f;
	public float armourRatio{get; protected set;}

	float armagelLevel;
	public float maxArmagelLevel = 50f;
	public float armagelUsageRate = 5f;

	//transforms and variables for torso rotation
	public Transform torsoBone;
	public float torsoRotationRate=20f;
	public float torsoAbsMaxRotation=90f;
	public float torsoRotation;//{get;protected set;}

	//variables for mech movement
	public float mechTurnRate=10f;
	public float throttleLevel{get; set;}
	float maxThrottle=1f;
	float minThrottle=-0.25f;


	// Use this for initialization
	void Start () {
	mechMotor = GetComponent <CharacterMotor>();
	animations = GetComponentInChildren<Animation>();
	mechWeapons=GetComponentInChildren<WeaponController>();
	mechWeapons.myController=this;
	isDead=false;
	
	throttleLevel=0.5f;
	torsoRotation=0;
	armourRatio=1;

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

	public void setThrottle(float newValue){


		throttleLevel=Mathf.Clamp(newValue,minThrottle,maxThrottle);

			
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

			animations.CrossFade ("Death",1f);

		}

	}

	public float GetThrottleLevel(){
		return throttleLevel;
	}

	public void TakeDamage(float damage){
		armourLevel-=damage;
		armourRatio=armourLevel/maxArmour;
	}

	void AssessDamage(){
		if(!isDead){
			if(armourLevel<=0f){
				isDead=true;
				mechWeapons.isDead=true;
				mechWeapons.StopAllWeapons ();
				allStop ();
				GameObject weh=(GameObject)Instantiate (deathExplosion,torsoBone.position,Quaternion.identity);
				weh.GetComponent<MechsplosionScript>().myParent=torsoBone;
			}
		}
		else{
			if(!animations.isPlaying){
				Destroy (gameObject);
			}
		}
	}
}
