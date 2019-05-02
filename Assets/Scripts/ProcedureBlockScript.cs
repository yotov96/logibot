using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class ProcedureBlockScript : MonoBehaviour {

	[Tooltip("The instruction board instance that this block will snap to automatically")]

	public RobotScript.Procedure procedure;

	[SerializeField]
	private bool isTemplate;
	public bool IsTemplate{ get { return isTemplate; } set { isTemplate = value; } }
	// Use this for initialization
	void Start()
	{
		procedure = new RobotScript.Procedure();
	}


}