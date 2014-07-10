using UnityEngine;
using System.Collections;

public class GarageCancelButton : ButtonScript {
	
	public override void onClick ()
	{
		GarageMenuScript.Instance.cancelOption();
	}
	
}
