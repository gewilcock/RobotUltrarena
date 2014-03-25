﻿using UnityEngine;
using System.Collections;

public class GlobalTargetList : MonoBehaviour {
	MechController[] mechList;
	float blip;

	public float maxScanRange=250f;
	// Use this for initialization
	void Start () {
		blip=Time.time+0.1f;


	}
	
	// Update is called once per frame
	void Update () {
		if((blip<Time.time)&&(blip>0)){
			mechList = GetComponentsInChildren<MechController>();
			//Debug.Log ("I found "+ mechList.Length.ToString ()+ " controllers!");

			blip=0;
		}
	}

	public Transform GetClosestTarget(Transform callerTransform){

		Transform bestTarget=null;
		
		float lastRange = maxScanRange;
		for(int i=0; i<mechList.Length;i++){

			if(mechList[i]!=null){
				Transform testTarget=mechList[i].transform;

				if(testTarget!=callerTransform){
				
					float testRange=Vector3.Distance(testTarget.position,callerTransform.position);
				
					if((testRange<lastRange)&&(testRange>0.5)){
						//if(!Physics.Raycast (callerTransform.position,testTarget.position-callerTransform.position,testRange-10f)){
							bestTarget=testTarget;
						//}
					}
					lastRange=testRange;
				}
				//else{Debug.Log ("Ignoring my own transform");}
			}

		}
		return bestTarget;

	
}
}