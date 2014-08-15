using UnityEngine;
using System.Collections;

public class UIHeatDisplayScript : MonoBehaviour {
	public ProgressBarScript heatBar;
	public Transform heatBarBase;
	public TextMesh infoText;
	WeaponController wControl;
	public Color overheatColor;
	Color baseColor;

	// Use this for initialization
	void Start () {
		transform.rotation=Quaternion.LookRotation (transform.position-transform.parent.transform.position);
		transform.localEulerAngles=new Vector3(transform.localEulerAngles.x,0,0);
		baseColor=heatBarBase.renderer.material.color;
		wControl=PlayerInputHandler.Instance.weaponController;
	}
	
	// Update is called once per frame
	void Update () {

		heatBar.fillRatio=wControl.heatRatio;

		if(wControl.isOverheating){
			heatBarBase.renderer.material.color=overheatColor;
			infoText.text="OVERHEATING";
		}
		else{
			if(wControl.heatRatio>0.75f){
				infoText.text="HEAT CRITICAL";
			}
			else{heatBarBase.renderer.material.color=baseColor;
				infoText.text="";
			}
		}
	}
}
