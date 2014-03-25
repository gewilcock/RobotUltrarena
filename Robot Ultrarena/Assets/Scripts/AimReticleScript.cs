using UnityEngine;
using System.Collections;

public class AimReticleScript : MonoBehaviour {
	public float reticleMaxDistance=6f;
	Ray aimRay;
	Camera HUDCamera;
	// Use this for initialization
	void Start () {
		HUDCamera=transform.parent.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		aimRay=HUDCamera.ScreenPointToRay(Input.mousePosition);
		transform.position=aimRay.GetPoint (reticleMaxDistance);
		transform.rotation=Quaternion.LookRotation (aimRay.direction);
	}
}
