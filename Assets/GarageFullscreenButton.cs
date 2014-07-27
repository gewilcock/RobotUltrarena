using UnityEngine;
using System.Collections;

public class GarageFullscreenButton : ButtonScript {

	public void Awake()
	{
		if((Application.platform != RuntimePlatform.WindowsWebPlayer) && (Application.platform != RuntimePlatform.OSXWebPlayer))
			Destroy (gameObject);

	}

	public override void onClick ()
	{
		//Screen.fullScreen = !Screen.fullScreen;
		Screen.SetResolution (1280, 720, true);
	}
	
}
