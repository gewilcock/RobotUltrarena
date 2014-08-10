using UnityEngine;
using System.Collections;

public class ChassisMenuControllerScript : MonoBehaviour
{
    public TextMesh Mechname;
    public TextMesh Mechstats;

    // Use this for initialization
    int mechIndex;

    void Start()
    {
        var spawner = GarageSpawnerScript.Spawner;
        
        if (spawner.wControl != null)
        {
            var mechChassis = PrefabLibraryScript.Instance.mechChassis;
            for (int i = 0; i < mechChassis.Length; i++)
            {
                if (spawner.wControl.gameObject.name.IndexOf(mechChassis[i].name) > -1)
                {
                    mechIndex = i;
                }
            }
        }
    }

    void cycleChassis(int shiftdirection)
    {
        mechIndex += shiftdirection;

        if (mechIndex < 0)
            mechIndex = PrefabLibraryScript.Instance.mechChassis.Length - 1;
        else if (mechIndex == PrefabLibraryScript.Instance.mechChassis.Length)
            mechIndex = 0;

        GameDataManagerScript.Instance.players[0].myMech = PrefabLibraryScript.Instance.mechChassis[mechIndex];

    }

    void updateText()
    {
        string namestring = GarageSpawnerScript.Spawner.mControl.gameObject.name;
        Mechname.text = namestring.Substring(0, namestring.Length - 7);
        Mechstats.text = PrefabLibraryScript.Instance.getDescription(Mechname.text) + "\n\n";
        Mechstats.text += "Armour: \t\t\t" + GarageSpawnerScript.Spawner.mControl.maxArmour.ToString() + "\n" +
            "Mass: \t\t\t" + (GarageSpawnerScript.Spawner.mControl.rigidbody.mass / 1000).ToString() + " tons\n" +
                "Max Speed: \t\t" + GarageSpawnerScript.Spawner.cMotor.movement.maxForwardSpeed.ToString() + " m/s\n" +
                "Max turn rate: \t\t" + GarageSpawnerScript.Spawner.mControl.mechTurnRate.ToString() + " degrees/s\n" +
                "Energy Cache: \t\t" + GarageSpawnerScript.Spawner.wControl.maxEnergyLevel.ToString() + " units\n" +
                "Cooling Rate: \t\t" + GarageSpawnerScript.Spawner.wControl.heatSinkRate.ToString() + " units/s\n\n" +
                "Weapon Slots: \t\t" + GarageSpawnerScript.Spawner.wControl.weaponList.Length.ToString() + "\n" +
                "Heavy Weapon Slots: \t" + GarageSpawnerScript.Spawner.wControl.heavyWeaponList.Length.ToString() + "\n" +
                "Module Slots: \t\t" + GarageSpawnerScript.Spawner.wControl.moduleList.Length.ToString();
    }
}
