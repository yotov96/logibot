using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class RobotScript : MonoBehaviour
{


	public enum Command
	{
		None,
		Forward,
		TurnLeft,
		TurnRight,
		Jump
	}
	public LinkedList<Command> commands = new LinkedList<Command>();
	public LinkedListNode<Command> currentCommand;
	private Animator animator;
	private Rigidbody rb;
	public GameObject target;
	public bool executing;
	public bool adjustingStartPosition;
	public bool start = true;
	public bool jumped;
	private int turned;


	private void Start()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += (sender, args) =>
			{
				transform.position = new Vector3(GameManager.GM.startTile.transform.position.x, GameManager.GM.startTile.transform.position.y + 2, GameManager.GM.startTile.transform.position.z);
			};
	}

	/**
	 * if we are in execution mode, resolve the current command and act accordingly
	 */
	private void Update()
	{
		if (executing)
		{
			ResolveCommand(currentCommand);
		}
		else if (adjustingStartPosition) // if not executing yet, we must be hovering towards the start tile
		{
			Vector3 targetPosition = new Vector3(GameManager.GM.startTile.transform.position.x, GameManager.GM.startTile.transform.position.y + 0.03f, GameManager.GM.startTile.transform.position.z);
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime);
			if (Vector3.Distance(transform.position, targetPosition) < .00001f)
			{
				adjustingStartPosition = false;
				GetComponent<Collider>().enabled = true;
			}
		}
	}

	/**
	 * method for waiting inbetween commands
	 */
	IEnumerator Wait(int seconds)
	{
		executing = false;
		yield return new WaitForSeconds(seconds);
		executing = true;

	}

	/**
	 * resolve the current command and act accordingly
	 */
	void ResolveCommand(LinkedListNode<Command> command)
	{
		switch (command.Value)
		{
			case Command.Forward:
				animator.SetBool("walking", true);
				if (target != null) // if we have a waypoint target (a tile), move towards the tile position with the Y axis offset in consideration, so that the robot appears on top of the level
				{
					Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y + 0.03f, target.transform.position.z);
					transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime);
					//if we reached the target position
					if (Vector3.Distance(transform.position, targetPosition) < .01f)
					{
						animator.SetBool("walking", false);
						//if we still have more commands to go through keep going
						if (currentCommand.Next != null)
						{
							target = null;
							currentCommand = currentCommand.Next;//next command
							StartCoroutine(Wait(1));
						}
						else
						{
							executing = false;
							if (target.GetComponent<TileScript>().isEnd)
							{
								target.GetComponent<TileScript>().isEnd = false;
								target = null;
								GameManager.GM.LevelSuccess();
								if (GameManager.GM.level < 5)
								{	
									GameManager.GM.level++;
									Invoke("NextLevel", 4f);
								}
							}
							else
							{
								GameManager.GM.LevelFail();
								Invoke("NextLevel", 4f);
							}
						}
					}
				}
				//if no target just go forward until we hit one
				else
				{
					transform.Translate(Vector3.forward * Time.deltaTime);
				}
				break;
			//turn 90 degrees left
			case Command.TurnLeft:
				transform.Rotate(Vector3.up * Time.deltaTime * 3f, -1);
				turned++;
				if (turned >= 90)
				{
					turned = 0;
					if (currentCommand.Next != null)
					{
						currentCommand = currentCommand.Next;
						StartCoroutine(Wait(1));
					}
					else
					{
						executing = false;
						GameManager.GM.LevelFail();
						Invoke("NextLevel", 4f);
					}
				}
				break;
			//turn 90 degrees right
			case Command.TurnRight:
				transform.Rotate(Vector3.up * Time.deltaTime * 3f, 1);
				turned++;
				if (turned >= 90)
				{
					turned = 0;
					if (currentCommand.Next != null)
					{
						currentCommand = currentCommand.Next;
						StartCoroutine(Wait(1));
					}
					else
					{
						executing = false;
						GameManager.GM.LevelFail();
						Invoke("NextLevel", 4f);	
					}
				}
				break;
			//turn 90 degrees right
			case Command.Jump:
				if (!jumped) // if not jumped, jump
				{
					rb.isKinematic = false;
					rb.AddForce(Vector3.up * 2, ForceMode.Impulse);
					rb.AddForce(transform.forward * 2, ForceMode.Impulse);
					jumped = true;
				}
				else if (target != null)
				{
					rb.isKinematic = true;
					Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y + 0.03f, target.transform.position.z);
					transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime);
					//if we reached the target position
					if (Vector3.Distance(transform.position, targetPosition) < .01f)
					{
						jumped = false;
						animator.SetBool("walking", false);
						//if we still have more commands to go through keep going
						if (currentCommand.Next != null)
						{
							//reset target
							target = null;
							currentCommand = currentCommand.Next;//next command
							StartCoroutine(Wait(1));
						}
						else
						{
							executing = false;
							if (target.GetComponent<TileScript>().isEnd)
							{
								target.GetComponent<TileScript>().isEnd = false;
								target = null;
								GameManager.GM.LevelSuccess();
								if (GameManager.GM.level < 5)
								{	
									GameManager.GM.level++;
									Invoke("NextLevel", 4f);
								}
							}
							else
							{
								GameManager.GM.LevelFail();
								Invoke("NextLevel", 4f);
							}
						}
					}
				}
				break;
		}
	}

	public void Stop()
	{
		executing = false;
		animator.SetBool("walking", false);
	}

	private void NextLevel()
	{
		GameManager.GM.SetUpLevel();
		GameManager.GM.HideCrossTick();
		GameManager.GM.SetUpRobot();
	}

	public void AddCommand(Command command)
	{
		if (commands.Count == 0)
		{		
			commands.AddFirst(command);
			currentCommand = commands.First;
		}
		else
			commands.AddLast(command);
	}
	
	public void AddCommand(Procedure procedure)
	{
		foreach (var command in procedure.procedureCommands)
		{
			AddCommand(command);
		}
	}

	public class Procedure
	{
		public LinkedList<Command> procedureCommands = new LinkedList<Command>();

		public void AddCommand(Command command)
		{
			if (procedureCommands.Count == 0)
				procedureCommands.AddFirst(command);
			else
				procedureCommands.AddLast(command);
		}
	}

}
