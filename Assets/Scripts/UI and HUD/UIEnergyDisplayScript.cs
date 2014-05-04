using UnityEngine;
using System.Collections;

public class UIEnergyDisplayScript : MonoBehaviour {

	public ProgressBarScript energyBar;
	public Transform energyBarBase;
	public TextMesh infoText;
	WeaponController wControl;
	public Color overheatColor;
	Color baseColor;
	
	// Use this for initialization
	void Start () {
		transform.rotation=Quaternion.LookRotation (transform.position-transform.parent.transform.position);
		transform.localEulerAngles=new Vector3(transform.localEulerAngles.x,0,0);
		baseColor=energyBarBase.renderer.material.color;
		wControl=MechInputHandler.playerController.GetComponentInChildren<WeaponController>();
	}
	
	// Update is called once per frame
	void Update () {
		
		energyBar.fillRatio=wControl.energyRatio;
		
		if(wControl.isOverheating){
			energyBarBase.renderer.material.color=overheatColor;
			infoText.text="OVERHEATING";
		}
		else{
			energyBarBase.renderer.material.color=baseColor;

			if(wControl.energyRatio<0.2f){
				infoText.text="ENERGY CRITICAL";

				if(wControl.energyRatio==0){
					infoText.text="ENERGY DEPLETED";
				}
			}

			else{
				infoText.text="";
			}
		}

	}
}
