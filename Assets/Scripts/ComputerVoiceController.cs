using UnityEngine;
using System.Collections;

public class ComputerVoiceController : MonoBehaviour {

	WeaponController wControl;
	MechController mControl;

	public AudioClip energyDepletedSound;
	public AudioClip armourCriticalSound;
	public AudioClip overheatingSound;
	public AudioClip ammoDepletedSound;

	float lastEnergy;
	void Awake () {

	}

	// Use this for initialization
	void Start () {
		mControl = MechInputHandler.playerController.GetComponentInChildren<MechController>();
		wControl = MechInputHandler.playerController.GetComponentInChildren<WeaponController>();
		lastEnergy = wControl.energyLevel;
	}
	
	// Update is called once per frame
	void Update () {

		if((wControl.energyLevel<lastEnergy) && (wControl.energyLevel<0))
			audio.PlayOneShot (energyDepletedSound);

		lastEnergy = wControl.energyLevel;
	}
}
