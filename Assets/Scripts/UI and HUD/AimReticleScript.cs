using UnityEngine;
using System.Collections;

public class AimReticleScript : MonoBehaviour {
	public float reticleMaxDistance=6f;
	Ray aimRay;
	Camera HUDCamera;
	MechInputHandler pInput;

	HUDTargetBracketScript currentBracket;
	int playerMask;
	int weaponPermeableMask;
	int aimPermeableMask;

	public Transform[] weaponRangeIndicator;
	
	public Material outOfArcColour;
	public Material baseColour;
	public Material validTargetColour;
	public Material noAmmoColour;
	public Material rechargeColour;
	public Material overheatColour;

	// Use this for initialization
	void Start () {
		playerMask = 1<<8;
		weaponPermeableMask = 1<<13;
		aimPermeableMask = 1<<15;
		playerMask = playerMask | weaponPermeableMask | aimPermeableMask;
		playerMask =~playerMask;

		currentBracket = null;

		HUDCamera=transform.parent.GetComponent<Camera>();
		pInput = MechInputHandler.playerController;

	}
	
	// Update is called once per frame
	void LateUpdate () {


		if(!pInput.wControl.inFireCone){
			renderer.material.color = outOfArcColour.color;

			for(int r=0; r<weaponRangeIndicator.Length; r++)
			{
				weaponRangeIndicator[r].renderer.material.color = outOfArcColour.color;
			}
		}
		else if(pInput.wControl.isOverheating)
		{
			renderer.material.color = overheatColour.color;
			
			for(int r=0; r<weaponRangeIndicator.Length; r++)
			{
				weaponRangeIndicator[r].renderer.material.color = overheatColour.color;
			}

		}
		else{

			if(pInput.isShootyTarget)
			{
				renderer.material.color = validTargetColour.color;
			}
			else 
			{
				renderer.material.color = baseColour.color;
			}

			for(int q=0; q<=2; q+=2)
			{
				for(int r=0; r<2; r++)
				{
					WeaponScript thisWeapon = pInput.wControl.weaponList[q+r];

					if(thisWeapon !=null)
					{
						if(thisWeapon.isOverheating)
							weaponRangeIndicator[q+r].renderer.material.color = overheatColour.color;
						else if((thisWeapon.ammoMaxLevel>0)&&(thisWeapon.ammoLevel==0))
							weaponRangeIndicator[q+r].renderer.material.color = noAmmoColour.color;
						else if(!thisWeapon.canFire)
							weaponRangeIndicator[q+r].renderer.material.color = rechargeColour.color;
						else if((pInput.wControl.aimRange[q/2]<=thisWeapon.weaponRange)&&(pInput.wControl.aimTag[q/2] == "DamageObject"))
							weaponRangeIndicator[q+r].renderer.material.color = validTargetColour.color;
						else
							weaponRangeIndicator[q+r].renderer.material.color = baseColour.color;
					}
					else
					{
						weaponRangeIndicator[q+r].renderer.material.color = baseColour.color;
					}
				}											
						

			}

		}


		if(pInput.isTargeting){
			createTargetingBracket();
		}

		aimRay=HUDCamera.ScreenPointToRay(Input.mousePosition);
		transform.position=aimRay.GetPoint (reticleMaxDistance);
		transform.rotation=Quaternion.LookRotation (aimRay.direction);
	}

	void createTargetingBracket(){

		bool spawnNew = false;
		RaycastHit targetingHit;
		bool hittarget=Physics.Raycast(aimRay, out targetingHit,MechInputHandler.MAX_AIM_DISTANCE,(playerMask));
		
		if(hittarget){
			Transform test = targetingHit.collider.transform;
			
			if(pInput.isShootyTarget){
				if(currentBracket!=null){
					Destroy (currentBracket.gameObject);
					currentBracket=null;
					spawnNew=true;
				}
				else{
					spawnNew=true;
				}
				
				if(spawnNew){
					
					GameObject weh=(GameObject)Instantiate(Resources.Load ("UITargetingBracket"),test.position,Quaternion.identity);
					currentBracket=weh.GetComponent<HUDTargetBracketScript>();
					currentBracket.myParent=test;
					currentBracket.playerMech=pInput.mControl.transform;
					pInput.wControl.lockedTarget = test;
				}
			}
		}
	}
}

