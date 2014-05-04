using UnityEngine;
using System.Collections;

public class AimReticleScript : MonoBehaviour {
	public float reticleMaxDistance=6f;
	Ray aimRay;
	Camera HUDCamera;
	MechInputHandler pInput;
	Color baseColor;
	HUDTargetBracketScript currentBracket;
	int playerMask;
	int weaponPermeableMask;
	int aimPermeableMask;

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
		baseColor=transform.renderer.material.color;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Color newColor;

		if(!pInput.wControl.inFireCone){
			newColor = new Color(Color.gray.r,Color.gray.g,Color.gray.b,0.2f);
		}
		else if(pInput.isShootyTarget){
			newColor = Color.red;
		}
		else{
			newColor = baseColor;
		}

		transform.renderer.material.color=newColor;

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
				}
			}
		}
	}
}

