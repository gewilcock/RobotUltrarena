using UnityEngine;
using System.Collections;

public class SimpleTank : MonoBehaviour {

	private float speed = 0.0f;
	private float maxSpeed = 8.0f;
	private float acceleration = 2.0f;

	private Quaternion oldHeading;
	private Quaternion newHeading;

	private float turningTime = 2.0f;
	public float turningCounter = 0.0f;

	private float drivingTime = 2.0f;
	public float drivingCounter = 0.0f;

	public enum Instructions
	{
		FullSpeedAhead,
		Turning,
		Stop,
		NewHeading
	}

	public Instructions currentInstruction = Instructions.NewHeading;
	private Instructions previousInstruction = Instructions.Stop;

	// Use this for initialization
	void Start () {
		newHeading = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateHeading();
		UpdateSpeed();

		transform.position += transform.forward * speed * Time.deltaTime;
	}

	private void UpdateSpeed()
	{
		if (currentInstruction == Instructions.FullSpeedAhead)
		{
			speed += acceleration * Time.deltaTime;
			if (speed > maxSpeed)
				speed = maxSpeed;

			drivingCounter += Time.deltaTime;
			if (drivingCounter >= drivingTime)
			{
				previousInstruction = Instructions.FullSpeedAhead;
				currentInstruction = Instructions.NewHeading;
			}
		}
		else if (currentInstruction == Instructions.Stop)
		{
			speed -= acceleration * Time.deltaTime;
			if (speed < 0.0f)
				speed = 0.0f;
		}
		else if (currentInstruction == Instructions.Turning)
		{
			if (speed > maxSpeed / 2.0f)
			{
				speed -= acceleration * Time.deltaTime;
			}
			else if (speed < maxSpeed / 2.0f)
			{
				speed += acceleration  * Time.deltaTime;
			}
		}
	}

	private void UpdateHeading()
	{
		if (currentInstruction == Instructions.NewHeading)
		{
			var direction = transform.rotation.eulerAngles.y + Random.Range(-55.0f, 55.0f);
			newHeading = Quaternion.Euler(270.0f, direction, 0.0f);
			oldHeading = transform.rotation;

			previousInstruction = currentInstruction;
			currentInstruction = Instructions.Turning;
			turningCounter = 0.0f;
		}
		else if (currentInstruction == Instructions.Turning)
		{
			transform.rotation = Quaternion.Slerp(oldHeading, newHeading, turningCounter / turningTime);
			turningCounter += Time.deltaTime;
			if (turningCounter >= turningTime)
			{
				previousInstruction = currentInstruction;
				currentInstruction = Instructions.FullSpeedAhead;
				drivingCounter = 0.0f;
			}

		}
	}
}
