using UnityEngine;
using System.Collections;

public class GarageSpawnerScript : MonoBehaviour {

	GameObject newMech;
	GameObject menu;
	public WeaponController wControl;
	public MechController mControl;
	public CharacterMotor cMotor;

	public static GarageSpawnerScript Spawner;

	void Awake()
	{
		Spawner = this;
	}

	// Use this for initialization
	void Start () {

	}

	public void spawnMech()
	{
		if(newMech){GameObject.Destroy(newMech);}

		MechData[] players = GameDataManagerScript.Instance.players;

		MechData weh = players[0];			

		newMech = (GameObject)Instantiate(weh.mechPrefab,transform.position,transform.rotation);			

		newMech.transform.parent = transform;
				
		wControl = newMech.GetComponent<WeaponController>();
		mControl = newMech.GetComponent<MechController>();
		cMotor = newMech.GetComponent<CharacterMotor>();
		
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
					GameObject newGun=(GameObject)Instantiate (weh.myHeavyWeapons[w],wControl.HWMounts[w].transform.position,wControl.gameObject.transform.rotation);
					
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

	// Update is called once per frame
	void Update () {
		if(newMech){
			transform.Rotate (0,20*Time.deltaTime,0);

			wControl.aimPoint = (wControl.transform.forward*500f);
		}
	}
}
