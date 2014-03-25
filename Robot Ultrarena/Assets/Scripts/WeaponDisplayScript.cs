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

	Color baseColor;
	public Color overheatColor;
	public Color cannotFireColor;


	// Use this for initialization
	void Start () {

		wControl=GameObject.Find ("PlayerMech").GetComponentInChildren<WeaponController>().weaponList[weaponIndex];
		if(wControl==null){
			Destroy (gameObject);
		}
		else{
			transform.rotation=Quaternion.LookRotation (transform.position-transform.parent.transform.position);

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
			infoText.text="";
		}
	}
}
