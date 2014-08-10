using UnityEngine;
using System.Collections;

public class PlaceholderMenuScript : MonoBehaviour {
	int selectedMech=0;
	int throttleSetting=0;
	int hWSetting=0;
	int moduleSetting=0;
	string hwDesc = "Dumbfire Rockets guide themselves to where your targeting reticle is aimed on launch.";
	string spDesc = "TOGGLED SYSTEM. Force Barriers block a portion of damage from each weapon hit, but at an equivalent energy cost. Recharge is slowed while active.";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch(hWSetting){
		case 0: hwDesc = "Dumbfire Missiles guide themselves to where your reticle was aimed as they were launched."; break;
		case 1: hwDesc = "Homing missiles require time to lock on, but will adjust their path to track moving targets. Position and keep your reticle over a target and hold the Heavy Weapon trigger button to lock on."; break;
		}

		switch(moduleSetting){
		case 0: spDesc = "TOGGLED SYSTEM. Force Barriers block a portion of damage from each weapon hit, but at an equivalent energy cost. Recharge is slowed while active."; break;
		case 1: spDesc = "ACTIVE SYSTEM. Jump jets propel you upwards, and offer jet-boosted turning while airborne. Jets have a limited charge, but will recharge when idle."; break;
		case 2: spDesc = "TOGGLED SYSTEM. ArmaGel Regen allows you to regenerate armour continuously over time when active. It has a limited usage, and will not recharge when depleted. Energy recharge is slowed when active."; break;
		case 3: spDesc = "ACTIVE SYSTEM. Emergency Battery boosts energy cache recharge rate using its own reserves. This module's capacity recharges, but slowly."; break;
		case 4: spDesc = "ACTIVE SYSTEM. Active Cooling increases mech cooling by flushing reactor coolant and replenishing it from its own stores. Will not recharge once depleted.";break;
		}
	}

	void OnGUI(){
		GUI.Label (new Rect(200,80,300,50),"Select Mech Chassis");
		selectedMech=GUI.SelectionGrid(new Rect(200,100,300,50),selectedMech,new string[]{"Deathbringer (Heavy)","Dervish (Light)"},2);

		GUI.Label (new Rect(200,160,300,50),"Select Throttle Setting");
		throttleSetting=GUI.SelectionGrid(new Rect(200,180,300,50),throttleSetting,new string[]{"Absolute (FPS style)","Adjust Manually"},2);

		GUI.Label (new Rect(200,240,300,50),"Select Heavy Weapon 1");
		hWSetting=GUI.SelectionGrid(new Rect(200,260,300,50),hWSetting,new string[]{"Dumb Missiles","Homing Missiles"},2);

		GUI.Label (new Rect(200,310,480,50),hwDesc);
		GUI.Label (new Rect(200,360,300,50),"Select Special Module");
		moduleSetting=GUI.SelectionGrid(new Rect(200,380,750,50),moduleSetting,new string[]{"Force Barrier","Jump jets","ArmaGel Regenerator","Emergency Battery","Active Cooling"},5);

		GUI.Label (new Rect(200,430,700,50),spDesc);
		if(GUI.Button (new Rect(350,490,100,50),"Start Game!")){

			if(throttleSetting==0){
			GameDataManagerScript.Instance.playerOptions.useAbsoluteThrottle=true;
			}
			else{
				GameDataManagerScript.Instance.playerOptions.useAbsoluteThrottle=false;
			}

			GameDataManagerScript.Instance.players[0].myHeavyWeapons[0]=PrefabLibraryScript.Instance.getHeavyWeapon(hWSetting);
			GameDataManagerScript.Instance.players[0].myModules[0]=PrefabLibraryScript.Instance.getAbilityModule(moduleSetting);
			GameDataManagerScript.Instance.players[0].myMech=PrefabLibraryScript.Instance.getMechChassis(selectedMech);

			Application.LoadLevel (1);
		}
	}
}
