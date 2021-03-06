﻿using UnityEngine;
using System.Collections;

public class GarageMenuScript : MonoBehaviour
{
    public static GarageMenuScript Instance;

    Ray cursorRay;
    int mask = 1 << 9;

    int selectedMenu = 0;

    public GameObject[] menus;
    //0 = Chassis
    //1 = Weapons hub
    //2 = Normal weapons
    //3 = Heavy weapons
    //4 = Modules

    int[] menusizes = new int[5];

    // Use this for initialization
    void Start()
    {
        Instance = this;

        GarageSpawnerScript.Spawner.spawnMech();

        menus[selectedMenu].BroadcastMessage("updateText");
    }


    // Update is called once per frame
    void Update()
    {
        cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit buttonhit;
        bool hit = Physics.Raycast(cursorRay, out buttonhit, Mathf.Infinity, mask);

        if (hit)
        {
            buttonhit.collider.BroadcastMessage("doHover");
            if (Input.GetMouseButtonDown(0))
            {
                buttonhit.collider.BroadcastMessage("doButtonDown");
            }
            if (Input.GetMouseButtonUp(0))
            {
                buttonhit.collider.BroadcastMessage("doClick");
            }
        }
    }

    public void shiftMenuPage(bool shiftBack = false)
    {
        int shiftdirection;
        if (shiftBack)
            shiftdirection = -1;
        else
            shiftdirection = 1;

        switch (selectedMenu)
        {
            case 0: menus[0].BroadcastMessage("cycleChassis", shiftdirection); break;
            case 1: menus[1].BroadcastMessage("cycleWeapons", shiftdirection); break;
            case 2: break;
            case 3: break;
            case 4: break;

        }

        GarageSpawnerScript.Spawner.spawnMech();
        menus[selectedMenu].BroadcastMessage("updateText");
    }

    public void acceptOption()
    {
        menus[selectedMenu].gameObject.SetActive(false);
        selectedMenu++;
        menus[selectedMenu].gameObject.SetActive(true);
        menus[selectedMenu].BroadcastMessage("updateText", SendMessageOptions.DontRequireReceiver);
    }

    public void cancelOption()
    {
        if (selectedMenu > 0)
        {
            menus[selectedMenu].gameObject.SetActive(false);
            selectedMenu--;
            menus[selectedMenu].gameObject.SetActive(true);
			menus[selectedMenu].BroadcastMessage("updateText", SendMessageOptions.DontRequireReceiver);
        }
    }

}
