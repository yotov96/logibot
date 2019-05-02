
using UnityEngine;
using VRTK;

public class TileHoverScript : MonoBehaviour
{

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("robot"))
		{
			RobotScript rs = other.gameObject.GetComponent<RobotScript>(); // get the robot script
			if (rs.commands.Count < 1)
			{
				GameManager.GM.SetUpRobot();
			}
			else
			{
				if (TileScript.firstCollision) // if this is the robot's first collision with a tile, hover to starting tile
				{
					other.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
					other.gameObject.GetComponent<Rigidbody>().isKinematic = true; // disable robot physics since we are moving it manually from now on
					other.GetComponent<Collider>().enabled = false;
					TileScript.firstCollision = false;
					rs.adjustingStartPosition = true;
				}
			}
		}
	}

}