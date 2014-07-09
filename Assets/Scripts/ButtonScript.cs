using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	public float baseHoverAlpha = 0.35f;
	public float hoverAlpha = 0.35f;
	public bool isToggled = false;
	public bool buttonDown = false;
	Color baseColor;

	// Use this for initialization
	void Start () {
		baseColor = transform.renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isToggled)
			hoverAlpha = Mathf.Clamp (hoverAlpha - 2*Time.deltaTime,baseHoverAlpha,1f);
		else 
			hoverAlpha = 1;


	}

	void LateUpdate () {
		if(!buttonDown)
			transform.renderer.material.color = new Color(baseColor.r,baseColor.g,baseColor.b,hoverAlpha);
		else
			transform.renderer.material.color = new Color(Color.white.r,Color.white.g,Color.white.b,hoverAlpha);
		}

	public void doClick(){

		buttonDown = false;
		
		onClick();
	}


	void doHover()
	{
		hoverAlpha = 1; 
	}

	void doButtonDown()
	{
		buttonDown = true;
	}

	public virtual void onClick(){}

}
