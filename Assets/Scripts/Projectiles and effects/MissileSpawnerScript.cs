using UnityEngine;
using System.Collections;

public class MissileSpawnerScript : MonoBehaviour {

	public int missilesToSpawn = 6;
	public GameObject missileProjectile;
	public float launchRadius=0.4f;
	public float totalLaunchTime=0.5f;
	public Transform lockedTarget;
	public bool isLocking;

	float nextLaunch;
	float launchTimeSlice;
	int missilesLaunched;
	WeaponController wControl;
	float angleSlices;

	// Use this for initialization
	void Start () {

		wControl = transform.parent.transform.parent.transform.GetComponent<WeaponScript>().myController;
		launchTimeSlice=totalLaunchTime/missilesToSpawn;
		nextLaunch=Time.time+0.2f;
		angleSlices=360f/(missilesToSpawn+1);
	}
	
	// Update is called once per frame
	void Update () {
		if((nextLaunch<=Time.time)&&(missilesLaunched<=missilesToSpawn)){
			Vector3 launchPosition = transform.position;
			GameObject m = (GameObject)Instantiate (missileProjectile,launchPosition,transform.rotation);
			if(!isLocking){
				m.GetComponent<MissileScript>().homingPoint=wControl.aimPoint;
			}
			else{
				m.GetComponent<MissileScript>().homingTransform=lockedTarget;
			}
			Physics.IgnoreCollision(m.collider, wControl.collider);
			missilesLaunched++;
			nextLaunch=Time.time+launchTimeSlice;

			if(missilesLaunched>missilesToSpawn){Destroy(this.gameObject);}
		}

	}
	
}
