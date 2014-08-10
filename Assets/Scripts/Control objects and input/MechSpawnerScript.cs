using UnityEngine;
using System.Collections;
using System;

public class MechSpawnerScript : MonoBehaviour
{
    public Transform playerStartingLocation;
    public GameObject PlayerCameraPrefab;
    public GameObject HUDObject;


    // Use this for initialization
    void Awake()
    {

    }

    void Start()
    {
        if (playerStartingLocation == null)
            throw new Exception("Player starting location not set");

        SpawnPlayers();

    }

    public void SpawnPlayers()
    {

        MechData[] players = GameDataManagerScript.Instance.players;

        foreach (MechData mechData in players)
        {
            GameObject newMech;

            if (mechData.isPlayer)
            {
                Instantiate(PlayerCameraPrefab, Vector3.zero, Quaternion.identity);
                newMech = (GameObject)Instantiate(mechData.myMech, playerStartingLocation.position, playerStartingLocation.rotation);
                MechInputHandler handler = (MechInputHandler)newMech.AddComponent("MechInputHandler");

                handler.absThrottle = GameDataManagerScript.Instance.playerOptions.useAbsoluteThrottle;

                Invoke("SpawnHUD", 0.1f);
                UpdatePerimeterWalls(newMech);
            }
            else
            {
                newMech = (GameObject)Instantiate(mechData.myMech, new Vector3(UnityEngine.Random.Range(30, 2200), 500, UnityEngine.Random.Range(30, 2200)), Quaternion.identity);
                newMech.AddComponent("MechAIHandler");

            }
            RaycastHit groundhit;
            Physics.Raycast(newMech.transform.position, Vector3.down, out groundhit, 1000f);
            newMech.transform.position = groundhit.point;

            WeaponController weaponController = newMech.GetComponent<WeaponController>();

            AddLightWeapons(mechData, weaponController);

            AddHeavyWeapons(mechData, weaponController);

            AddModules(mechData, weaponController);

        }
    }

    private void UpdatePerimeterWalls(GameObject newMech)
    {
        GameObject.Find("NorthWall").GetComponent<BarrierScript>().player = newMech.transform;
        GameObject.Find("SouthWall").GetComponent<BarrierScript>().player = newMech.transform;
        GameObject.Find("EastWall").GetComponent<BarrierScript>().player = newMech.transform;
        GameObject.Find("WestWall").GetComponent<BarrierScript>().player = newMech.transform;
    }

    private void AddModules(MechData mechData, WeaponController wControl)
    {
        for (int w = 0; w < mechData.myModules.Length; w++)
        {
            if (mechData.myModules[w] != null)
            {
                if (w < wControl.moduleList.Length)
                {
                    GameObject newModule = (GameObject)Instantiate(mechData.myModules[w], wControl.transform.position, Quaternion.identity);

                    newModule.transform.parent = wControl.transform;
                    wControl.moduleList[w] = newModule.GetComponent<AbilityModuleScript>();
                    wControl.moduleList[w].wControl = wControl;
                }
            }
        }
    }

    private void AddHeavyWeapons(MechData mechData, WeaponController wControl)
    {
        for (int w = 0; w < mechData.myHeavyWeapons.Length; w++)
        {
            if (mechData.myHeavyWeapons[w] != null)
            {
                if (w < wControl.HWMounts.Length)
                {
                    GameObject newGun = (GameObject)Instantiate(mechData.myHeavyWeapons[w], wControl.HWMounts[w].transform.position, Quaternion.identity);

                    newGun.transform.parent = wControl.HWMounts[w].transform;

                    wControl.heavyWeaponList[w] = newGun.GetComponent<WeaponScript>();
                }
            }
        }
    }

    private void AddLightWeapons(MechData mechData, WeaponController wControl)
    {
        for (int w = 0; w < mechData.myWeapons.Length; w++)
        {
            if (mechData.myWeapons[w] != null)
            {

                GameObject newGun = (GameObject)Instantiate(mechData.myWeapons[w], Vector3.zero, Quaternion.identity);

                int pivotIndex;
                float mountRotation;

                if (w < wControl.weaponsPerGroup)
                {
                    pivotIndex = 0;
                    mountRotation = w + wControl.weaponsPerGroup - 1;
                }
                else
                {
                    pivotIndex = 1;
                    mountRotation = w + (2 * wControl.weaponsPerGroup) - 1;
                }


                newGun.transform.parent = wControl.aimPivots[pivotIndex].transform;
                newGun.transform.localPosition = Vector3.zero;
                newGun.transform.localEulerAngles = new Vector3(0, 0, -180 + (180 * mountRotation));

                wControl.weaponList[w] = newGun.GetComponent<WeaponScript>();

            }
        }
    }

    void SpawnHUD()
    {
        GameObject HUD = (GameObject)Instantiate(HUDObject, Camera.main.transform.position, Camera.main.transform.rotation);
        HUD.transform.parent = Camera.main.transform;

    }
}
