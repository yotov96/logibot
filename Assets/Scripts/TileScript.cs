using System.Collections;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;


/**
 * Script that controls Tile interactions and behaviour
 */
public class TileScript : MonoBehaviour
{
	public static bool firstCollision = true;
	public bool settingup = true; //set up
	public Vector3 positionTarget;
	private RobotScript rs;
	public bool isEnd;
	
	/**
	 * Called when something (most likely the robot) enters the collider of this tile
	 */
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("robot"))
		{
			rs = other.GetComponent<RobotScript>();
			if (rs.start && !firstCollision) // if we collided with starting tile
			{
				rs.start = false;
				other.gameObject.transform.position = transform.position;  //snap the robot to the tile
				other.gameObject.transform.Translate(new Vector3(0, 0.03f, 0)); // and lift it up so that theyre not overlapping
				Invoke("StartExecuting", 2);
				//enable colliders
				foreach (var collider in GameManager.GM.outsideWalls.GetComponents<Collider>()) 
				{
					collider.enabled = true;
				}
				foreach (var collider in  GameManager.GM.walls.GetComponents<Collider>())
				{
					collider.enabled = true;
				}
			}
			else if(rs.executing) //otherwise robot is already underway, set its waypoint to this
			{
				rs.target = gameObject;
			}
		}
	}

	void StartExecuting()
	{ 			
		rs.executing = true; //start execution 
	}

	/**
	 * upon creation set the positon target at the level floor (Y axis 0)
	 */
	private void Start()
	{
		positionTarget = new Vector3(transform.position.x,0.5f, transform.position.z);
	}

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
	}
}
