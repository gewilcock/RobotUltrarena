using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class GameDataManagerScript : MonoBehaviour {

	public static GameDataManagerScript GameDataManager {get; private set;}

	public MechData[] players;



	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (gameObject);
		GameDataManager=this;


	}





}

[Serializable]
public class MechData {

	public bool isPlayer;
	public string myName;
	public GameObject myMech;
	public GameObject[] myWeapons;

}
