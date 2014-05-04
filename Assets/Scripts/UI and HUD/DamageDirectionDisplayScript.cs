using UnityEngine;
using System.Collections;

public class DamageDirectionDisplayScript : MonoBehaviour {
	public Color damageColor;

	public Renderer top;
	public Renderer bottom;
	public Renderer left;
	public Renderer right;

	MechController mControl;
	Vector2 damageDirection;

	// Use this for initialization
	void Start () {
		mControl=MechInputHandler.playerController.GetComponentInChildren<MechController>();
		top.material.color = new Color(damageColor.r,damageColor.g,damageColor.b,0);
		bottom.material.color = new Color(damageColor.r,damageColor.g,damageColor.b,0);
		left.material.color = new Color(damageColor.r,damageColor.g,damageColor.b,0);
		right.material.color = new Color(damageColor.r,damageColor.g,damageColor.b,0);

	}
	
	// Update is called once per frame
	void Update () {
		damageDirection.x = Mathf.Clamp (mControl.damageVector.x,-1,1);
		damageDirection.y = Mathf.Clamp (mControl.damageVector.z,-1,1);

		damageDirection = Quaternion.Euler(0,0,-mControl.torsoRotation)*damageDirection;

		if(damageDirection.y>0){
			top.material.color = new Color(damageColor.r,damageColor.g,damageColor.b,damageDirection.y);
		}
		if(damageDirection.y<0){
			bottom.material.color = new Color(damageColor.r,damageColor.g,damageColor.b,Mathf.Abs (damageDirection.y));
		}
		if(damageDirection.x>0){
			right.material.color = new Color(damageColor.r,damageColor.g,damageColor.b,damageDirection.x);
		}
		if(damageDirection.x<0){
			left.material.color = new Color(damageColor.r,damageColor.g,damageColor.b,Mathf.Abs (damageDirection.x));
		}

	}
}
