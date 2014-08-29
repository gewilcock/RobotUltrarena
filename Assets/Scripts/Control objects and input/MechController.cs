using UnityEngine;
using System.Collections;

public class MechController : MonoBehaviour {

	//This script is responsible for mech movement and steering, health and animation management.

	public GameObject damageSmokeEffect;
	public GameObject reactorBreachEffect;
	public GameObject deathExplosion;
	public GameObject debrisPrefab;

	//Reference scripts
	private CharacterMotor mechMotor;
	private CharacterController cControl;

	private WeaponController wControl;
	public Animation animations;
	public Transform AIAimPoint;
	public Transform cockpit;

	//armour and damage processing variables

	public bool isDead{get;protected set;}

	public float armourLevel = 100f;
	public float maxArmour = 100f;
	public float armourRatio{get; protected set;}

	public Vector3 damageVector;
	public float totalDamage;

	public bool hasTakenDamage=false;
	public bool isSmoking = false;

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
	public float maxDampingVelocity=20f;
	float momentumDampening;
	public float speedModifier;
	public float turnModifier;
	float lastVelocity = 0;
	public float collisionDamageSpeedRatio = 0.5f;
	public float collisionDamageMax = 10f;

	float spawnProtection = 1f;

	//Specialised variables for modules
	public bool isShielded{get;set;}
	public float damageDampening{get;set;} //used for any damage dampening - armour, shields, whatever.

	// Use this for initialization
	void Start () {
	
	mechMotor = GetComponent <CharacterMotor>();

	animations = GetComponentInChildren<Animation>();
	wControl=GetComponentInChildren<WeaponController>();
	wControl.myController=this;

	cControl=transform.GetComponent<CharacterController>();
	isDead=false;
	
	throttleLevel=0;
	torsoRotation=0;
	armourRatio=1;

	speedModifier = 0;
	turnModifier = 0;
	damageVector = Vector3.zero;

	spawnProtection +=Time.time;
			
	animations.Play ("Stationary");

	}
	
	// Update is called once per frame
	void Update () {

		hasTakenDamage=false;
		if(cControl.isGrounded)
		{
			mechMotor.inputMoveDirection = (transform.rotation*Vector3.forward*throttleLevel*(1+speedModifier));
		}
		CollisionDamageCheck();
		AssessDamage();
		ApplyAnimations();

	}


	public void rotateTorso(float direction){

		torsoRotation=Mathf.Clamp (torsoRotation+(direction*torsoRotationRate)*Time.deltaTime,-torsoAbsMaxRotation,torsoAbsMaxRotation);
		torsoBone.localEulerAngles = new Vector3(torsoRotation,270f,0f);
		
	}

	public void rotateMech(float direction){

		if(cControl.isGrounded)
		{
			momentumDampening = 1-(cControl.velocity.magnitude/maxDampingVelocity);

			transform.Rotate (new Vector3(0f,mechTurnRate*Time.deltaTime*direction*momentumDampening*(1+turnModifier)));
		}
	}

	public void setThrottle(float newValue){


		throttleLevel=Mathf.Clamp(newValue,minThrottle,maxThrottle);

			
	}

	public void allStop(){
		
		throttleLevel=0f;

	}

	public void ApplyAnimations(){
	
		if(!isDead){
			if(cControl.isGrounded){
				if(mechMotor.movement.velocity.sqrMagnitude==0f){
					animations.CrossFade ("Stationary",0.5f);
				}
				else{
				if(!animations.IsPlaying("Run")){animations.CrossFade ("Run",2f);}
					animations["Run"].speed = 0.3f+mechMotor.movement.velocity.sqrMagnitude/Mathf.Pow (mechMotor.movement.maxForwardSpeed,2);
				}
			}
			else{
				animations.Stop ();
			}
		}
		else{

			animations.CrossFade ("Death",1f);

		}

	}

	public float GetThrottleLevel(){
		return throttleLevel;
	}

	public void TakeDamage(CollisionDataContainer cd){


		if(isShielded){
			GameObject weh = (GameObject)Instantiate (Resources.Load ("ShieldImpactEffect"),cd.hitPoint,Quaternion.LookRotation (cd.hitNormal));
			float damageSize = Mathf.Clamp (cd.damageValue,1f,10f);
			weh.transform.localScale = new Vector3(damageSize,damageSize,1);
			weh.transform.parent=this.transform;
			wControl.energyLevel -= cd.damageValue * damageDampening;
		}

		totalDamage += cd.damageValue*(1-damageDampening);
		damageVector+=Vector3.Normalize(transform.InverseTransformPoint(cd.hitPoint))*Mathf.Clamp (0,3,cd.damageValue*(1-damageDampening));
		//damageVector+=transform.InverseTransformPoint(cd.hitPoint);

		armourLevel-= cd.damageValue*(1-damageDampening);

			
		hasTakenDamage=true;


			
	}

	public void CollisionDamageCheck()
	{
		if(spawnProtection<Time.time)
		{
			float currentV = cControl.velocity.magnitude;
			float deltaV = currentV-lastVelocity;
			float hitDelta = mechMotor.movement.maxForwardSpeed*collisionDamageSpeedRatio;

			if(deltaV < 0)
				if(Mathf.Abs (deltaV)>=hitDelta)
				{
					float collisionDamageTaken = collisionDamageMax*(Mathf.Abs (deltaV)-hitDelta)/hitDelta;

					TakeDamage (new CollisionDataContainer(collisionDamageTaken,cControl.velocity,cControl.velocity.normalized));

				}		
					
			lastVelocity = currentV;
		}
	}

	void AssessDamage(){


		damageVector = Vector3.Lerp (damageVector,Vector3.zero,Time.deltaTime*2);
		totalDamage = Mathf.Lerp (totalDamage,1,Time.deltaTime*2);



		if(!isDead){

			if(armourLevel<20 && !isSmoking)
			{
				GameObject smoke = (GameObject)Instantiate (damageSmokeEffect);
				smoke.transform.parent = torsoBone;
				smoke.transform.localPosition = new Vector3(0,0,0);
				isSmoking = true;
			}

			armourRatio=armourLevel/maxArmour;

			if(armourLevel<=0f){
				isDead=true;
				wControl.isDead=true;
				wControl.StopAllWeapons ();
				allStop ();
				GameObject weh=(GameObject)Instantiate (reactorBreachEffect,AIAimPoint.position,Quaternion.identity);
				weh.GetComponent<MechsplosionScript>().myParent=AIAimPoint;
			}
		}
		else{
			if(!animations.isPlaying){

				Instantiate (deathExplosion,AIAimPoint.position,Quaternion.identity);
				GameObject DebrisPrefab = (GameObject)Instantiate (debrisPrefab,AIAimPoint.position,transform.rotation*Quaternion.AngleAxis(180,Vector3.up));
				DebrisPrefab.BroadcastMessage("explode",AIAimPoint.position);

				Destroy (gameObject);
			}
		}
	}
	
	public void JumpMech(float thrust){


		if(thrust > 0)
		{

			mechMotor.SetGrounded(true);

			mechMotor.jumping.baseHeight = thrust;
			mechMotor.inputJump = true;
		}
		else{

			mechMotor.inputJump = false;
			/*if(!mechMotor.IsGrounded())
			{
				mechMotor.SetGrounded(true);

			}*/

		}
		//mechMotor.SetJumping(true);	
	}
}
