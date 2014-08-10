using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class GameDataManagerScript : MonoBehaviour
{

    public static GameDataManagerScript Instance { get; private set; }

    public PlayerPreferences playerOptions;

    public MechData[] players;



    // Use this for initialization
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else { Debug.Log("Destroying manager because an instance already exists"); Destroy(gameObject); }

    }





}

[Serializable]
public class MechData
{

    public bool isPlayer;
    public string myName;
    public GameObject myMech;
    public GameObject[] myWeapons;
    public GameObject[] myHeavyWeapons;
    public GameObject[] myModules;

}

[Serializable]
public class PlayerPreferences
{
    public bool useAbsoluteThrottle = false;
}
