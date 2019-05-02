using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class PanelButton : MonoBehaviour
{
	private VRTK_InteractableObject io;
	public bool level1;
	public bool level2;
	public bool level3;
	public bool level4;
	public bool level5;

	private void Start()
	{
		io = GetComponent<VRTK_InteractableObject>();
		io.InteractableObjectUsed += onClick;
	}



	public void onClick(object sender, InteractableObjectEventArgs args)
	{
		if (level1)
		{
			GameManager.GM.level = 1;

		}
		if (level2)
		{
			GameManager.GM.level = 2;

		}
		if (level3)
		{
			GameManager.GM.level = 3;

		}
		if (level4)
		{
			GameManager.GM.level = 4;

		}
		if (level5)
		{
			GameManager.GM.level = 5;
		}
		GameManager.GM.SetUpLevel();
		GameManager.GM.SetUpRobot();	
	}
}
