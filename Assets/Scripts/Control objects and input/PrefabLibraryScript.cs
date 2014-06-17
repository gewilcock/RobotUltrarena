using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class PrefabLibraryScript : MonoBehaviour {
	public static PrefabLibraryScript library;

	public GameObject[] mechChassis;
	public GameObject[] weapons;
	public GameObject[] heavyWeapons;
	public GameObject[] abilityModules;

	JSONNode descriptionJSON;

	void Awake(){
		library=this;
		TextAsset descriptionFile = Resources.Load<TextAsset>("descriptions");
		descriptionJSON = JSON.Parse (descriptionFile.text);

	}

	public GameObject getMechChassis(int index){

		if((mechChassis.Length>0)&&(index<mechChassis.Length)&&(index>=0)){
			return mechChassis[index];
		}
		else{Debug.LogError ("Mech list is empty or request index is out of range."); return null;}

	}

	public GameObject getWeapon(int index){
		if((weapons.Length>0)&&(index<weapons.Length)&&(index>=0)){
			return weapons[index];
		}
		else{Debug.LogError ("Weapon list is empty or request index is out of range.");return null;}
	}

	public GameObject getHeavyWeapon(int index){
		if((heavyWeapons.Length>0)&&(index<heavyWeapons.Length)&&(index>=0)){
			return heavyWeapons[index];
		}
		else{Debug.LogError ("Heavy weapon list is empty or request index is out of range.");return null;}
	}

	public GameObject getAbilityModule(int index){
		if((abilityModules.Length>0)&&(index<abilityModules.Length)&&(index>=0)){
			return abilityModules[index];
		}
		else{Debug.LogError ("Module list is empty or request index is out of range.");return null;}
	}

	public string getDescription(string key)
	{
		return descriptionJSON[key];
	}
}
