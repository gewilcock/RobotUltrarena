using UnityEngine;
using System.Collections;

public class WaterTriggerScript : MonoBehaviour {
	public float waterSpeedDampening;
	public float waterTurnDampening;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		MechController mControl = other.GetComponent<MechController>();
		if(mControl)
		{
			mControl.speedModifier -= waterSpeedDampening;
			mControl.turnModifier -= waterTurnDampening;
		}
	}

	void OnTriggerExit(Collider other)
	{
		MechController mControl = other.GetComponent<MechController>();
		if(mControl)
		{
			mControl.speedModifier += waterSpeedDampening;
			mControl.turnModifier += waterTurnDampening;
		}

	}
}
