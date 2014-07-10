using UnityEngine;
using System.Collections;

public class GarageConfirmButton : ButtonScript {

	public override void onClick ()
	{
		GarageMenuScript.garageMenuController.acceptOption();
	}

}
