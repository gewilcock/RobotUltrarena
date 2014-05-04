using UnityEngine;
using System.Collections;

public class PrefabLibraryScript : MonoBehaviour {
	public static PrefabLibraryScript library;

	public GameObject[] mechChassis;
	public GameObject[] weapons;
	public GameObject[] heavyWeapons;
	public GameObject[] abilityModules;

	void Awake(){
		library=this;
	}

	public GameObject getMechChassis(int index){

		if((mechChassis.Length>0)&&(index<mechChassis.Length)){
			return mechChassis[index];
		}
		else{Debug.LogError ("Mech list is empty or request index is out of range."); return null;}
	}

	public GameObject getWeapon(int index){
		if((weapons.Length>0)&&(index<weapons.Length)){
			return weapons[index];
		}
		else{Debug.LogError ("Weapon list is empty or request index is out of range.");return null;}
	}

	public GameObject getHeavyWeapon(int index){
		if((heavyWeapons.Length>0)&&(index<heavyWeapons.Length)){
			return heavyWeapons[index];
		}
		else{Debug.LogError ("Heavy weapon list is empty or request index is out of range.");return null;}
	}

	public GameObject getAbilityModule(int index){
		if((abilityModules.Length>0)&&(index<abilityModules.Length)){
			return abilityModules[index];
		}
		else{Debug.LogError ("Module list is empty or request index is out of range.");return null;}
	}
}
