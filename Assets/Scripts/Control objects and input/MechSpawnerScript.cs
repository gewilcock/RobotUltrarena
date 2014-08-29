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

	public void SpawnPlayers()
	{

		MechData[] players = GameDataManagerScript.GameDataManager.players;
		float angleSlice = (Mathf.PI*2)/players.Length;
		float startOffset = Random.Range(0f,2*Mathf.PI);

		for(int i = 0; i<players.Length;i++)
		{
			float startAngle = startOffset+(angleSlice*i);

			MechData weh = players[i];
			GameObject newMech;

			Vector3 startPoint = new Vector3 (1000+(900*Mathf.Cos (startAngle)),500,1000+(900*Mathf.Sin (startAngle)));
			RaycastHit groundhit;
			Physics.Raycast (startPoint,Vector3.down,out groundhit,1000f);
			startPoint=groundhit.point+Vector3.up;


			if(weh.isPlayer){
				Instantiate (PlayerCameraPrefab,Vector3.zero,Quaternion.identity);
				newMech = (GameObject)Instantiate(weh.myMech,startPoint,Quaternion.identity);
				MechInputHandler handler = (MechInputHandler)newMech.AddComponent ("MechInputHandler");

				handler.absThrottle = GameDataManagerScript.GameDataManager.playerOptions.useAbsoluteThrottle;

				Invoke ("SpawnHUD",0.1f);

			}
			else{
				newMech = (GameObject)Instantiate (weh.myMech,startPoint,Quaternion.identity);
				newMech.AddComponent ("MechAIHandler");

			}


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

			for(int w=0; w<weh.myHeavyWeapons.Length;w++){
				if(weh.myHeavyWeapons[w]!=null){
					if(w<wControl.HWMounts.Length){
					GameObject newGun=(GameObject)Instantiate (weh.myHeavyWeapons[w],wControl.HWMounts[w].transform.position,Quaternion.identity);

					newGun.transform.parent=wControl.HWMounts[w].transform;
					
					wControl.heavyWeaponList[w]=newGun.GetComponent<WeaponScript>();
					}
				}
			}

			for(int w=0; w<weh.myModules.Length;w++){
				if(weh.myModules[w]!=null){
					if(w<wControl.moduleList.Length){
						GameObject newModule=(GameObject)Instantiate (weh.myModules[w],wControl.transform.position,Quaternion.identity);
						
						newModule.transform.parent=wControl.transform;
						wControl.moduleList[w]=newModule.GetComponent<AbilityModuleScript>();
						wControl.moduleList[w].wControl=wControl;
					}
				}
			}



		}
	}

	void SpawnHUD(){
		GameObject HUD = (GameObject)Instantiate (HUDObject,Camera.main.transform.position,Camera.main.transform.rotation);
		HUD.transform.parent=Camera.main.transform;

	}
}
