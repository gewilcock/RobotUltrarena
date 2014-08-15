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
        else 
        { 
            Debug.Log("Destroying manager because an instance already exists"); 
            Destroy(gameObject); 
        }

    }

    void Update()
    {
        var isWinner = true;

        foreach (var player in players)
        {
            if (!player.isDead && !player.isPlayer)
            {
                isWinner = false;
            }
        }

        if (isWinner)
        {

        }
    }

}

[Serializable]
public class MechData
{
    public bool isPlayer;
    public string callSign;
    public GameObject mechPrefab;
    public GameObject[] myWeapons;
    public GameObject[] myHeavyWeapons;
    public GameObject[] myModules;
    public bool isDead;
}

[Serializable]
public class PlayerPreferences
{
    public bool useAbsoluteThrottle = false;
}
