using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class InstructionBlock : MonoBehaviour {

    [Tooltip("The instruction board instance that this block will snap to automatically")]
    public InstructionBoard instructionBoard;
    private VRTK_InteractableObject interact;
    [Tooltip("The instruction that this block provides to the robot")]
    public RobotScript.Command robotCommand;

    [SerializeField]
    private bool isTemplate;
    public bool IsTemplate{ get { return isTemplate; } set { isTemplate = value; } }
    // Use this for initialization
    void Start()
    {
        interact = GetComponent<VRTK_InteractableObject>();
        interact.InteractableObjectUsed += Used;
    }

    private void Used(object sender, InteractableObjectEventArgs e)
    {
        if(isTemplate && instructionBoard && instructionBoard.GetNextEmptySlot())//see if there's actually a free slot before instantiating anything
        {
            GameObject clone = Instantiate(gameObject);
            clone.GetComponent<Rigidbody>().isKinematic = false;
            clone.GetComponent<Rigidbody>().useGravity = true;
            InstructionBlock newBlock = clone.GetComponent<InstructionBlock>();
            newBlock.IsTemplate = false;
            instructionBoard.AutoSnapToBoard(newBlock);
        }
    }
}
