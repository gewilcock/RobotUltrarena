using UnityEngine;
using System.Collections;

public class EquipmentSlotButtonScript : ButtonScript {
	public EquipMenuControllerScript myMenu;
	public TextMesh weaponNameLabel;
	public int libraryIndex =-1;

	void Awake()
	{
		weaponNameLabel = transform.GetComponentInChildren<TextMesh>();

		transform.rotation=Quaternion.LookRotation (transform.position-Camera.main.transform.position);
		transform.localEulerAngles=new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y,0);

	}

	public override void onClick () {
		myMenu.toggleButtons (this);
	}
}
