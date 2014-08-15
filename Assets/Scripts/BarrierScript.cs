using UnityEngine;
using System.Collections;

public class BarrierScript : MonoBehaviour {

	public Transform player;
    public float alpha;
	
	public enum Placement
	{
		North,
		South,
		West,
		East
	}
	
	public Placement placement;
	public float maximumDistance = 40.0f;
	public float minimumDistance = 20.0f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	private float CalculateAlpha(float barrierposition, float playerposition)
	{
		float alpha = 0.0f;
		float distance = 0.0f;

		if (barrierposition > playerposition)
			distance = barrierposition - playerposition;
		else
			distance = playerposition - barrierposition;

		if (distance <= minimumDistance)
		{
			alpha = 1.0f;
		}
		else if (distance > minimumDistance && distance < maximumDistance)
		{
			alpha = 1.0f - (distance - minimumDistance) / (maximumDistance - minimumDistance);
		}
		
		return alpha;
	}
	
	// Update is called once per frame
	void Update () {
		
		switch (placement)
		{
		case Placement.North:
			alpha = CalculateAlpha(transform.position.z, player.position.z);
			break;
		case Placement.South:
			alpha = CalculateAlpha(transform.position.z, player.position.z);
			break;
		case Placement.West:
			alpha = CalculateAlpha(transform.position.x, player.position.x);
			break;
		case Placement.East:
			alpha = CalculateAlpha(transform.position.x, player.position.x);
			break;
		}
		
		renderer.material.color = new Color(1.0f, 1.0f, 1.0f, alpha);
	}
}
