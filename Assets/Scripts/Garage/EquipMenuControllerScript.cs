using UnityEngine;
using System.Collections;

public class EquipMenuControllerScript : MonoBehaviour
{
    public TextMesh Weaponname;
    public TextMesh Weaponstats;

    public MeshFilter weaponModel;

    Vector3 originLocation;

    public EquipmentSlotButtonScript[] mechWeapons;
    public EquipmentSlotButtonScript[] mechHeavyWeapons;
    public EquipmentSlotButtonScript[] mechModules;

    // Use this for initialization
    void OnEnable()
    {
        hideInactiveSlots();
        populateSlots();

        toggleButtons(mechWeapons[0]);
        updateText();
    }

    void Update()
    {
        weaponModel.transform.RotateAround(weaponModel.transform.parent.position, Vector3.up, 45 * Time.deltaTime);
    }

    void updateText()
    {
        populateSlots();

        weaponModel.mesh = null;
        string namestring = "";
        string statistics = "";

        for (int w = 0; w < mechWeapons.Length; w++)
        {
            if (mechWeapons[w].isToggled)
            {
                if (GarageSpawnerScript.Spawner.wControl.weaponList[w] != null)
                {
                    namestring = GarageSpawnerScript.Spawner.wControl.weaponList[w].gameObject.name;

                    statistics = "Range: \t\t\t\t" + GarageSpawnerScript.Spawner.wControl.weaponList[w].weaponRange + "m\n";
                    statistics += "Refire Rate: \t\t\t" + GarageSpawnerScript.Spawner.wControl.weaponList[w].refireTime + "s\n";
                    statistics += "Heat per shot: \t\t\t" + GarageSpawnerScript.Spawner.wControl.weaponList[w].heatPerShot + "\n";
                    statistics += "Cooldown rate: \t\t\t" + GarageSpawnerScript.Spawner.wControl.weaponList[w].heatSinkRate + "/s\n";
                    if (GarageSpawnerScript.Spawner.wControl.weaponList[w].ammoMaxLevel > 0)
                    {
                        statistics += "Max Ammunition: \t\t" + GarageSpawnerScript.Spawner.wControl.weaponList[w].ammoMaxLevel + "\n";
                        statistics += "Ammo per Shot: \t\t\t" + GarageSpawnerScript.Spawner.wControl.weaponList[w].ammoPerShot + "\n";
                    }
                    if (GarageSpawnerScript.Spawner.wControl.weaponList[w].energyPerShot > 0)
                        statistics += "Energy per Shot: \t\t\t" + GarageSpawnerScript.Spawner.wControl.weaponList[w].energyPerShot + "\n";

                    weaponModel.mesh = GarageSpawnerScript.Spawner.wControl.weaponList[w].GetComponentInChildren<MeshFilter>().mesh;

                    weaponModel.transform.parent.rotation = Quaternion.identity;
                    weaponModel.transform.rotation = Quaternion.identity;
                    weaponModel.transform.localPosition = new Vector3(0, 0, -2f);

                }
                else
                    weaponModel.mesh = null;
            }
        }

        for (int w = 0; w < mechHeavyWeapons.Length; w++)
        {
            if (mechHeavyWeapons[w].isToggled)
            {
                if (GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w] != null)
                {
                    namestring = GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].gameObject.name;

                    statistics = "Range: \t\t\t\t" + GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].weaponRange + "m\n";
                    statistics += "Refire Rate: \t\t\t" + GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].refireTime + "s\n";
                    statistics += "Heat per shot: \t\t\t" + GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].heatPerShot + "\n";
                    statistics += "Cooldown rate: \t\t\t" + GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].heatSinkRate + "/s\n";
                    if (GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].ammoMaxLevel > 0)
                    {
                        statistics += "Max Ammunition: \t\t" + GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].ammoMaxLevel + "\n";
                        statistics += "Ammo per Shot: \t\t\t" + GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].ammoPerShot + "\n";
                    }
                    if (GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].energyPerShot > 0)
                        statistics += "Energy per Shot: \t\t\t" + GarageSpawnerScript.Spawner.wControl.weaponList[w].energyPerShot + "\n";

                    weaponModel.mesh = GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].GetComponentInChildren<MeshFilter>().mesh;

                    weaponModel.transform.parent.rotation = Quaternion.identity;
                    weaponModel.transform.rotation = Quaternion.identity;
                    weaponModel.transform.localPosition = Vector3.zero;
                }
                else
                    weaponModel.mesh = null;

            }

        }

        for (int w = 0; w < mechModules.Length; w++)
        {
            if (mechModules[w].isToggled)
            {
                if (GarageSpawnerScript.Spawner.wControl.moduleList[w] != null)
                {
                    namestring = GarageSpawnerScript.Spawner.wControl.moduleList[w].gameObject.name;

                    if (GarageSpawnerScript.Spawner.wControl.moduleList[w].isToggledModule)
                        statistics = "TOGGLED MODULE\nEnable and disable manually.\n\n";
                    else
                        statistics = "ACTIVE MODULE\nHold down button to apply effect.\n\n";

                    statistics += "Recharges:\t\t\t" + GarageSpawnerScript.Spawner.wControl.moduleList[w].canRecharge + "\n";
                    if (GarageSpawnerScript.Spawner.wControl.moduleList[w].energyCost > 0)
                        statistics += "Energy cost:\t\t\t" + GarageSpawnerScript.Spawner.wControl.moduleList[w].energyCost + "/sec\n";
                    if (GarageSpawnerScript.Spawner.wControl.moduleList[w].abilityCapacity > 0)
                        statistics += "Total operational capacity:\t\t" + GarageSpawnerScript.Spawner.wControl.moduleList[w].abilityCapacity + " secs\n";

                }

            }
        }

        if (string.IsNullOrEmpty(namestring))
            Weaponname.text = "EMPTY";
        else
            Weaponname.text = namestring.Substring(0, namestring.Length - 7);

        Weaponstats.text = PrefabLibraryScript.Instance.getDescription(Weaponname.text) + "\n\n" + statistics;


    }

    void hideInactiveSlots()
    {
        for (int w = 0; w < mechWeapons.Length; w++)
        {
            mechWeapons[w].gameObject.SetActive(false);

            if (w < GarageSpawnerScript.Spawner.wControl.weaponList.Length)
            {
                mechWeapons[w].gameObject.SetActive(true);
                mechWeapons[w].myMenu = this;
            }


        }
        for (int h = 0; h < mechHeavyWeapons.Length; h++)
        {
            mechHeavyWeapons[h].gameObject.SetActive(false);

            if (h < GarageSpawnerScript.Spawner.wControl.heavyWeaponList.Length)
            {
                mechHeavyWeapons[h].gameObject.SetActive(true);
                mechHeavyWeapons[h].myMenu = this;
            }

        }
        for (int m = 0; m < mechModules.Length; m++)
        {
            mechModules[m].gameObject.SetActive(false);
            if (m < GarageSpawnerScript.Spawner.wControl.moduleList.Length)
            {
                mechModules[m].gameObject.SetActive(true);
                mechModules[m].myMenu = this;
            }
        }
    }

    void populateSlots()
    {
        AddLightWeapons();
        
        AddHeavyWeapons();

        AddAbilityModules();
    }
    
    private void AddLightWeapons()
    {
        for (int w = 0; w < GarageSpawnerScript.Spawner.wControl.weaponList.Length; w++)
        {
            for (int l = 0; l < PrefabLibraryScript.Instance.weapons.Length; l++)
            {
                if (GarageSpawnerScript.Spawner.wControl.weaponList[w] != null)
                {
                    if (GarageSpawnerScript.Spawner.wControl.weaponList[w].gameObject.name.IndexOf(PrefabLibraryScript.Instance.weapons[l].name) > -1)
                    {
                        mechWeapons[w].libraryIndex = l;
                        mechWeapons[w].weaponNameLabel.text = GarageSpawnerScript.Spawner.wControl.weaponList[w].weaponName;
                        mechWeapons[w].weaponNameLabel.characterSize = 0.15f;
                    }
                }
                else
                {
                    mechWeapons[w].weaponNameLabel.text = "EMPTY";
                    mechWeapons[w].weaponNameLabel.characterSize = 0.08f;
                }
            }
        }
    }
    
    private void AddHeavyWeapons()
    {
        for (int w = 0; w < GarageSpawnerScript.Spawner.wControl.heavyWeaponList.Length; w++)
        {
            for (int l = 0; l < PrefabLibraryScript.Instance.heavyWeapons.Length; l++)
            {
                if (GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w] != null)
                {
                    if (GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].gameObject.name.IndexOf(PrefabLibraryScript.Instance.heavyWeapons[l].name) > -1)
                    {
                        mechHeavyWeapons[w].libraryIndex = l;
                        mechHeavyWeapons[w].weaponNameLabel.text = GarageSpawnerScript.Spawner.wControl.heavyWeaponList[w].weaponName;
                        mechHeavyWeapons[w].weaponNameLabel.characterSize = 0.15f;
                    }
                }
                else
                {
                    mechHeavyWeapons[w].weaponNameLabel.text = "EMPTY";
                    mechHeavyWeapons[w].weaponNameLabel.characterSize = 0.08f;
                }
            }
        }
    }
    
    private void AddAbilityModules()
    {
        for (int m = 0; m < GarageSpawnerScript.Spawner.wControl.moduleList.Length; m++)
        {
            for (int l = 0; l < PrefabLibraryScript.Instance.abilityModules.Length; l++)
            {
                if (GarageSpawnerScript.Spawner.wControl.moduleList[m] != null)
                {
                    if (GarageSpawnerScript.Spawner.wControl.moduleList[m].gameObject.name.IndexOf(PrefabLibraryScript.Instance.abilityModules[l].name) > -1)
                    {
                        mechModules[m].libraryIndex = l;
                        mechModules[m].weaponNameLabel.text = GarageSpawnerScript.Spawner.wControl.moduleList[m].moduleName;
                        mechModules[m].weaponNameLabel.characterSize = 0.15f;
                    }
                }
                else
                {
                    mechModules[m].weaponNameLabel.text = "EMPTY";
                    mechModules[m].weaponNameLabel.characterSize = 0.08f;
                }
            }
        }
    }

    public void cycleWeapons(int shiftdirection)
    {
        for (int w = 0; w < mechWeapons.Length; w++)
        {
            if (mechWeapons[w].isToggled)
            {
                mechWeapons[w].libraryIndex += shiftdirection;

                if (mechWeapons[w].libraryIndex > PrefabLibraryScript.Instance.weapons.Length - 1) 
				{ 
					mechWeapons[w].libraryIndex = -1; 
				}

                if (mechWeapons[w].libraryIndex < -1) 
				{ 
					mechWeapons[w].libraryIndex = PrefabLibraryScript.Instance.weapons.Length - 1; 
				}

                GameDataManagerScript.GameDataManager.players[0].myWeapons[w] = PrefabLibraryScript.Instance.getWeapon(mechWeapons[w].libraryIndex);
            }
        }

        for (int w = 0; w < mechHeavyWeapons.Length; w++)
        {
            if (mechHeavyWeapons[w].isToggled)
            {
                mechHeavyWeapons[w].libraryIndex += shiftdirection;

                if (mechHeavyWeapons[w].libraryIndex > PrefabLibraryScript.Instance.heavyWeapons.Length - 1) { mechHeavyWeapons[w].libraryIndex = -1; }
                if (mechHeavyWeapons[w].libraryIndex < -1) { mechHeavyWeapons[w].libraryIndex = PrefabLibraryScript.Instance.heavyWeapons.Length - 1; }


                GameDataManagerScript.GameDataManager.players[0].myHeavyWeapons[w] = PrefabLibraryScript.Instance.getHeavyWeapon(mechHeavyWeapons[w].libraryIndex);
            }
        }

        for (int w = 0; w < mechModules.Length; w++)
        {
            if (mechModules[w].isToggled)
            {

                mechModules[w].libraryIndex += shiftdirection;

                if (mechModules[w].libraryIndex > PrefabLibraryScript.Instance.abilityModules.Length - 1) { mechModules[w].libraryIndex = -1; }
                if (mechModules[w].libraryIndex < -1) { mechModules[w].libraryIndex = PrefabLibraryScript.Instance.abilityModules.Length - 1; }


                GameDataManagerScript.GameDataManager.players[0].myModules[w] = PrefabLibraryScript.Instance.getAbilityModule(mechModules[w].libraryIndex);
            }
        }

    }

    public void toggleButtons(EquipmentSlotButtonScript newButton)
    {
        for (int w = 0; w < mechWeapons.Length; w++)
        {
            mechWeapons[w].isToggled = false;

        }

        for (int w = 0; w < mechHeavyWeapons.Length; w++)
        {
            mechHeavyWeapons[w].isToggled = false;

        }

        for (int m = 0; m < mechModules.Length; m++)
        {
            mechModules[m].isToggled = false;
        }

        newButton.isToggled = true;

        updateText();
    }

}
