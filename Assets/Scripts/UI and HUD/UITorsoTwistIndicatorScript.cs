using UnityEngine;
using System.Collections;

public class UITorsoTwistIndicatorScript : MonoBehaviour {
	MechController mControl;
	// Use this for initialization
	void Start () {
		mControl=MechInputHandler.playerController.GetComponentInChildren<MechController>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.localEulerAngles = new Vector3(90f,mControl.torsoRotation,0);
	}
}
