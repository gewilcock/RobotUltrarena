using UnityEngine;
using System.Collections;

public class WeaponDisplayScript : MonoBehaviour {

	ProgressBarScript heatDisplay; 
	ProgressBarScript refireDisplay;
	Transform backplate;
	TextMesh infoText;
	TextMesh nameText;
	public int weaponIndex;
	public WeaponScript wControl;
	public bool isHeavyWeapon;

	Transform lockOnReticle;

	Color baseColor;
	public Color overheatColor;
	public Color cannotFireColor;
	public Color ammoDepletedColor;


	// Use this for initialization
	void Start () {
		lockOnReticle = null;

		if(isHeavyWeapon){
			var heavyWeaponList = MechInputHandler.playerController.GetComponentInChildren<WeaponController>().heavyWeaponList;

			if(weaponIndex >= heavyWeaponList.Length)
				Destroy (gameObject);
			else
				wControl = heavyWeaponList[weaponIndex];
		}
		else{
			var weaponList = MechInputHandler.playerController.GetComponentInChildren<WeaponController>().weaponList;

			if(weaponIndex >= weaponList.Length)
				Destroy (gameObject);
			else
				wControl = weaponList[weaponIndex];
		}

		if(wControl==null)
		{
			Destroy (gameObject);
		}
		else{
			transform.rotation=Quaternion.LookRotation (transform.position-transform.parent.transform.position);
			transform.localEulerAngles=new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y,0);

			heatDisplay = transform.FindChild ("UIWHeatBar").GetComponent<ProgressBarScript>();
			refireDisplay = transform.FindChild ("UIWRefireBar").GetComponent<ProgressBarScript>();
			backplate = transform.FindChild ("UIWeaponDisplayBack").transform;

			baseColor=backplate.renderer.material.color;

			infoText=transform.FindChild ("UIWeaponInfoText").GetComponent<TextMesh>();
			nameText=transform.FindChild ("UIWeaponNameText").GetComponent<TextMesh>();

			nameText.text=wControl.weaponName;
		}
	}
	
	// Update is called once per frame
	void Update () {
		ScaleBars();
		ColorBars();
		UpdateText();
		CheckForLock();
	}

	void ScaleBars(){
		heatDisplay.fillRatio=wControl.heatRatio;
		refireDisplay.fillRatio=wControl.refireRatio;

	}

	void ColorBars(){
		Color newColor;
		if(wControl.isOverheating){
			newColor=overheatColor;
		}
		else if((wControl.ammoLevel == 0) && wControl.ammoMaxLevel > 0)
		{
			newColor=ammoDepletedColor;
		}
		else if(!wControl.canFire){
			newColor=cannotFireColor;
		}
		else{
			newColor=baseColor;
		}

		if(!wControl.isActive){
			newColor= new Color(newColor.r,newColor.g,newColor.b,0.1f);
		}
		
		backplate.renderer.material.color=newColor;

	}

	void UpdateText(){

		if(wControl.isOverheating){
			infoText.text="OVERHEATING";
		}
		else if(wControl.ammoMaxLevel>0){
			infoText.text=wControl.ammoLevel.ToString ();
		}
		else{
			infoText.text=Mathf.Clamp (Mathf.RoundToInt(wControl.myController.energyLevel/wControl.energyPerShot),0,1000).ToString ();
		}
	}

	void CheckForLock(){

		if((wControl.requiresLock)&&(wControl.triggered)&&(wControl.myController.lockedTarget)&&(wControl.lockOnRatio > 0)){
			if(lockOnReticle==null){lockOnReticle = ((GameObject)Instantiate(Resources.Load ("MissileLockBrackets"))).transform;}
			else{
					lockOnReticle.position=wControl.myController.lockedTarget.position+new Vector3(0,5,0);
					float lockRatio = Mathf.Clamp (wControl.lockOnRatio*200f,12f,200f);
					
				if(lockRatio<=15f){lockOnReticle.renderer.material.color=Color.yellow;}

					lockOnReticle.localScale = new Vector3(lockRatio,lockRatio,1);
					lockOnReticle.LookAt (Camera.main.transform.position);
			}
		}
		else{
			if(lockOnReticle!=null){
				Destroy(lockOnReticle.gameObject);
				lockOnReticle=null;
			}
		}
	}
}
