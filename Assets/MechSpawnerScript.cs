using UnityEngine;
using System.Collections;

public class MechSpawnerScript : MonoBehaviour {

	public GameObject PlayerCameraPrefab;
	public GameObject HUDObject;

	// Use this for initialization
	void Awake () {

	}

	void Start(){
	
		SpawnPlayers ();

	}

	// Update is called once per frame
	void Update () {
	
	}

	void SpawnPlayers(){

		MechData[] players = GameDataManagerScript.GameDataManager.players;

		for(int i =0; i<players.Length;i++){

			MechData weh = players[i];
			GameObject newMech;

			if(weh.isPlayer){
				Instantiate (PlayerCameraPrefab,Vector3.zero,Quaternion.identity);
				newMech = (GameObject)Instantiate(weh.myMech,new Vector3(50,500,50),Quaternion.identity);
				newMech.AddComponent ("MechInputHandler");

				Invoke ("SpawnHUD",0.1f);

			}
			else{
				newMech = (GameObject)Instantiate (weh.myMech,new Vector3(UnityEngine.Random.Range (30,1100),500,UnityEngine.Random.Range(30,1100)),Quaternion.identity);
				newMech.AddComponent ("MechAIHandler");
			}
			RaycastHit groundhit;
			Physics.Raycast (newMech.transform.position,Vector3.down,out groundhit,1000f);
			newMech.transform.position=groundhit.point;

			WeaponController wControl = newMech.GetComponent<WeaponController>();

			for(int w=0; w<weh.myWeapons.Length;w++){
				if(weh.myWeapons[w]!=null){

					GameObject newGun=(GameObject)Instantiate (weh.myWeapons[w],Vector3.zero,Quaternion.identity);

					int pivotIndex;
					float mountRotation;

					if(w<wControl.weaponsPerGroup){
						pivotIndex=0;
						mountRotation=w+wControl.weaponsPerGroup-1;
					}
					else{
						pivotIndex=1;
						mountRotation=w+(2*wControl.weaponsPerGroup)-1;
					}


					newGun.transform.parent=wControl.aimPivots[pivotIndex].transform;
					newGun.transform.localPosition=Vector3.zero;
					newGun.transform.localEulerAngles = new Vector3(0,0,-180+(180*mountRotation));

					wControl.weaponList[w]=newGun.GetComponent<WeaponScript>();
				}
			}

		}
	}

	void SpawnHUD(){
		GameObject HUD = (GameObject)Instantiate (HUDObject,Camera.main.transform.position,Camera.main.transform.rotation);
		HUD.transform.parent=Camera.main.transform;

	}
}
