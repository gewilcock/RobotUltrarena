using UnityEngine;
using System.Collections;

public class DeathCameraScript : MonoBehaviour {
	float halfscreenw;
	public Transform playerMech;
	// Use this for initialization
	void OnEnable () {
		halfscreenw = Screen.width/2;
		Screen.showCursor=true;
	}
	
	// Update is called once per frame
	void Update () {
		if(playerMech!=null){
			transform.LookAt (playerMech.position);
		}
	}

	void OnGUI(){

		if(GUI.Button (new Rect(halfscreenw-50,Screen.height-150,100,50),"Restart")){
			Application.LoadLevel (0);
		}

	}
}
