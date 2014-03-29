using UnityEngine;
using System.Collections;

public class ProgressBarScript : MonoBehaviour {
	public float fillRatio{get;set;}
	public Color minColor;
	public Color maxColor;

	//Vector3 basePosition;
	Vector3 baseScale;
	Vector3 halfBase;
	// Use this for initialization
	void Start () {
		//basePosition=transform.localPosition;
		baseScale=transform.localScale;
		halfBase=transform.localScale/2;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(fillRatio*baseScale.x,transform.localScale.y,transform.localScale.z);
		transform.localPosition = new Vector3(((halfBase.x*fillRatio)-halfBase.x),transform.localPosition.y,transform.localPosition.z);
		renderer.material.color=Color.Lerp (minColor,maxColor,fillRatio);

	}
}
