using UnityEngine;
using System.Collections;

public class HUDThrottleScript : MonoBehaviour {
	MechController mController;
	ProgressBarScript throttleBar;
	// Use this for initialization
	void Start () {
		transform.rotation=Quaternion.LookRotation (transform.position-transform.parent.transform.position);
		transform.Rotate (0,0,90f);
		MechInputHandler yay = GameObject.Find ("PlayerMech").GetComponent<MechInputHandler>();
		mController=yay.mControl;

		throttleBar=transform.GetComponentInChildren <ProgressBarScript>();

	}
	
	// Update is called once per frame
	void Update () {
		throttleBar.fillRatio=mController.GetThrottleLevel();
	}

}
