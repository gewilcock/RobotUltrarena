using UnityEngine;
using System.Collections;

public class GaragePageButtonScript : ButtonScript {
	public bool isBackButton;

	public override void onClick ()
	{
		GarageMenuScript.Instance.shiftMenuPage (isBackButton);
	}
}
