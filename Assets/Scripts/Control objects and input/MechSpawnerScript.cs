using UnityEngine;
using System.Collections;

public class MechSpawnerScript : MonoBehaviour
{

    public GameObject PlayerCameraPrefab;
    public GameObject HUDObject;
	public Transform playerStartLocation;

    // Use this for initialization
    void Awake()
    {

    }

    void Start()
    {

        SpawnPlayers();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnPlayers()
    {

        MechData[] players = GameDataManagerScript.GameDataManager.players;

        for (int i = 0; i < players.Length; i++)
        {

            MechData weh = players[i];
            GameObject newMech;

            if (weh.isPlayer)
            {
                Instantiate(PlayerCameraPrefab, Vector3.zero, Quaternion.identity);
				newMech = (GameObject)Instantiate(weh.myMech, playerStartLocation.position, playerStartLocation.rotation);
                MechInputHandler handler = (MechInputHandler)newMech.AddComponent("MechInputHandler");

                handler.absThrottle = GameDataManagerScript.GameDataManager.playerOptions.useAbsoluteThrottle;

<<<<<<< HEAD
                Invoke("SpawnHUD", 0.1f);
=======
			}
			else{
				newMech = (GameObject)Instantiate (weh.myMech,new Vector3(UnityEngine.Random.Range (30,2200),500,UnityEngine.Random.Range(30,2200)),Quaternion.identity);
				newMech.AddComponent ("MechAIHandler");

			}
			RaycastHit groundhit;
			Physics.Raycast (newMech.transform.position,Vector3.down,out groundhit,1000f);
			newMech.transform.position=groundhit.point;
>>>>>>> upstream/master

				var northBarrier = GameObject.Find("NorthBarrier");
				northBarrier.GetComponent<BarrierScript>().player = Camera.main.transform;
				var southBarrier = GameObject.Find("SouthBarrier");
				southBarrier.GetComponent<BarrierScript>().player = Camera.main.transform;
				var eastBarrier = GameObject.Find("EastBarrier");
				eastBarrier.GetComponent<BarrierScript>().player = Camera.main.transform;
				var westBarrier = GameObject.Find("WestBarrier");
				westBarrier.GetComponent<BarrierScript>().player = Camera.main.transform;

            }
            else
            {
                newMech = (GameObject)Instantiate(weh.myMech, new Vector3(UnityEngine.Random.Range(30, 2200), 500, UnityEngine.Random.Range(30, 2200)), Quaternion.identity);
                newMech.AddComponent("MechAIHandler");
            }

            RaycastHit groundhit;
            Physics.Raycast(newMech.transform.position, Vector3.down, out groundhit, 1000f);
            newMech.transform.position = groundhit.point;

            WeaponController wControl = newMech.GetComponent<WeaponController>();

            for (int w = 0; w < weh.myWeapons.Length; w++)
            {
                if (weh.myWeapons[w] != null)
                {

                    GameObject newGun = (GameObject)Instantiate(weh.myWeapons[w], Vector3.zero, Quaternion.identity);

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

            for (int w = 0; w < weh.myHeavyWeapons.Length; w++)
            {
                if (weh.myHeavyWeapons[w] != null)
                {
                    if (w < wControl.HWMounts.Length)
                    {
                        GameObject newGun = (GameObject)Instantiate(weh.myHeavyWeapons[w], wControl.HWMounts[w].transform.position, Quaternion.identity);

                        newGun.transform.parent = wControl.HWMounts[w].transform;

                        wControl.heavyWeaponList[w] = newGun.GetComponent<WeaponScript>();
                    }
                }
            }

            for (int w = 0; w < weh.myModules.Length; w++)
            {
                if (weh.myModules[w] != null)
                {
                    if (w < wControl.moduleList.Length)
                    {
                        GameObject newModule = (GameObject)Instantiate(weh.myModules[w], wControl.transform.position, Quaternion.identity);

                        newModule.transform.parent = wControl.transform;
                        wControl.moduleList[w] = newModule.GetComponent<AbilityModuleScript>();
                        wControl.moduleList[w].wControl = wControl;
                    }
                }
            }

        }
    }

    void SpawnHUD()
    {
        GameObject HUD = (GameObject)Instantiate(HUDObject, Camera.main.transform.position, Camera.main.transform.rotation);
        HUD.transform.parent = Camera.main.transform;
    }
}
