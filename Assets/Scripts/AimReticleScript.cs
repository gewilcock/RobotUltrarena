using UnityEngine;
using System.Collections;

public class AimReticleScript : MonoBehaviour {
	public float reticleMaxDistance=6f;
	Ray aimRay;
	Camera HUDCamera;
	MechInputHandler pInput;
	Color baseColor;
	// Use this for initialization
	void Start () {
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

		aimRay=HUDCamera.ScreenPointToRay(Input.mousePosition);
		transform.position=aimRay.GetPoint (reticleMaxDistance);
		transform.rotation=Quaternion.LookRotation (aimRay.direction);
	}
}
