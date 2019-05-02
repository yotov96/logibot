using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
	public bool settingup;
	public bool unsettingup;
	public Vector3 positionTarget;


	/**
	 * if we are setting up, move up until we reach the level floor to craete the cool set up effect
	 */
	private void Update()
	{
		if (settingup)
		{
			transform.position = Vector3.MoveTowards(transform.position, positionTarget, Time.deltaTime * 2);
			if (Vector3.Distance(transform.position, positionTarget) < .001f)
				settingup = false;
		}

		if (unsettingup) // if changing level, move down
		{
			Vector3 unsetTarget = new Vector3(transform.position.x, -5, transform.position.z);
			transform.position = Vector3.MoveTowards(transform.position, unsetTarget, Time.deltaTime * 2);
			if (Vector3.Distance(transform.position, unsetTarget) < .001f)
			{
				unsettingup = false;
				Destroy(gameObject);
			}

		}
	}

	//when we collide with robot
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("robot"))
		{
			RobotScript rs = other.gameObject.GetComponent<RobotScript>(); // get the robot script
			if (rs.executing) // stop the robot, since it's game over
			{
				rs.Stop();
				GameManager.GM.LevelFail();
				Invoke("Retry", 4f);
			}
		}	
	}
	
	private void Retry()
	{
		GameManager.GM.SetUpLevel();
		GameManager.GM.HideCrossTick();
		GameManager.GM.SetUpRobot();
	}
}
