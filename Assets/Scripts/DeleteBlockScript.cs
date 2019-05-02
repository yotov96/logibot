
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DeleteBlockScript : MonoBehaviour
{
    private VRTK_InteractableObject io;
    
    private void Start()
    {
        io = GetComponent<VRTK_InteractableObject>();
        io.InteractableObjectUsed += onClick;
    }


    public void onClick(object sender, InteractableObjectEventArgs args)
    {

        GameManager.GM.ClearInstructions();
        GameManager.GM.robot.GetComponent<RobotScript>().commands = new LinkedList<RobotScript.Command>();
       

    }

    
}
