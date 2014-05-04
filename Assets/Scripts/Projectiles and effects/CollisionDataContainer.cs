using UnityEngine;
using System.Collections;

public class CollisionDataContainer {
	public float damageValue;
	public Vector3 hitPoint;
	public Vector3 hitNormal;

	public CollisionDataContainer(float dv, Vector3 hp, Vector3 hn){
		damageValue = dv;
		hitPoint = hp;
		hitNormal = hn;
	}
}
