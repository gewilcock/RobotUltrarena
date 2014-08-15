using UnityEngine;
using System.Collections;

public class UIHealthbarScript : MonoBehaviour {

	public ProgressBarScript healthBar;
	public Transform healthBarBase;
	public TextMesh infoText;
	MechController mControl;
	WeaponController wControl;
	public Color overheatColor;
	Color baseColor;
	
	// Use this for initialization
	void Start () {
		transform.rotation=Quaternion.LookRotation (transform.position-transform.parent.transform.position);
		transform.localEulerAngles=new Vector3(transform.localEulerAngles.x,0,0);
		baseColor=healthBarBase.renderer.material.color;
		mControl=PlayerInputHandler.Instance.GetComponentInChildren<MechController>();
		wControl=PlayerInputHandler.Instance.GetComponentInChildren<WeaponController>();
	}
	
	// Update is called once per frame
	void Update () {
		
		healthBar.fillRatio=mControl.armourRatio;
		
		if(wControl.isOverheating){
			healthBarBase.renderer.material.color=overheatColor;
		}
		else{
			healthBarBase.renderer.material.color=baseColor;
		}	

		infoText.text= Mathf.Ceil(mControl.armourLevel).ToString ();			
		
	}
}
