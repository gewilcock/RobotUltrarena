using UnityEngine;
using System.Collections;


public class HUDTargetBracketScript : MonoBehaviour {

	public Transform myParent{get;set;}
	MechController targetController;

	public Transform offscreenArrow;
	public Transform bracket;
	ProgressBarScript targetHealth;

	public Transform playerMech{protected get;set;}



	Vector3 bracketScale;
	Vector3 screenPoint;
	Camera maincamera;

	// Use this for initialization
	void Start () {
		targetController=myParent.GetComponent<MechController>();
		bracket=transform.FindChild ("Brackets");
		offscreenArrow=transform.FindChild ("HUDOffscreenArrow");
		maincamera=Camera.main;

		targetHealth = transform.GetComponentInChildren<ProgressBarScript>();

		transform.LookAt (maincamera.transform.position);
		bracketScale=bracket.localScale;
		bracket.localScale= new Vector3(200,200,1);
	}
	
	// Update is called once per frame
	void Update () {


		bracket.localScale=Vector3.Lerp (bracket.localScale,bracketScale,15*Time.deltaTime);


		if(myParent!=null){

			if(targetController.isDead){
				Destroy(this.gameObject);
			}
			else{
				transform.position=myParent.position;
				transform.LookAt (maincamera.transform.position);
				targetHealth.fillRatio=targetController.armourRatio;
			}

			screenPoint=maincamera.WorldToViewportPoint(transform.position);

			if((screenPoint.x<0)||(screenPoint.x>1)||(screenPoint.y<0)||(screenPoint.y>1)||(screenPoint.z<0)){

				offscreenArrow.gameObject.SetActive (true);

				Vector3 targetVector= targetController.transform.position-playerMech.position;

				float pointDirection= Mathf.Atan2 (targetVector.z,targetVector.x)*Mathf.Rad2Deg;;

				//Ray posray=maincamera.ViewportPointToRay(clampPoint);
				Ray posray=maincamera.ViewportPointToRay(new Vector3(0.5f,0.2f,0));


				offscreenArrow.position=posray.GetPoint (5f);
				offscreenArrow.rotation=Quaternion.Euler (new Vector3(90f,0f,pointDirection));





		}
			else{
				offscreenArrow.gameObject.SetActive(false);
			}

		}

	}

	/*void OnGUI(){
		if(myParent!=null){
			GUI.Label (new Rect(0,0,64,64),screenPoint.ToString());
		}
	}*/
}
