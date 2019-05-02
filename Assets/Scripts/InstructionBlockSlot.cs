using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class InstructionBlockSlot : MonoBehaviour {

    public InstructionBoard parentBoard;
    public VRTK_SnapDropZone snapDrop;
    public RobotScript.Command Command
    {
        get
        {
            GameObject block = snapDrop.GetCurrentSnappedObject(); 
            if (block && block.GetComponent<InstructionBlock>())
                return block.GetComponent<InstructionBlock>().robotCommand;
            else
                return RobotScript.Command.None;
        }
    }

    void Start () {
        snapDrop = GetComponent<VRTK_SnapDropZone>();

        VRTK_InteractableObject snapped;
        if(snapped = snapDrop.defaultSnappedInteractableObject)
        {
            if (GetComponentInParent<InstructionBoard>()!= null && GetComponentInParent<InstructionBoard>().isProcedureBoard)
            {
                ProcedureBlockScript block;
                if (block = snapped.gameObject.GetComponent<ProcedureBlockScript>())
                {
                    block.IsTemplate = true;
                }
            }
            else
            {
                InstructionBlock block;
                if (block = snapped.gameObject.GetComponent<InstructionBlock>())
                {
                    block.IsTemplate = true;
                }
            }
            snapDrop.ObjectUnsnappedFromDropZone += UnSnap;
        }
    }

    private void UnSnap(object sender, SnapDropZoneEventArgs e)
    {
        if (e.snappedObject != null)
        {
            if (GetComponentInParent<InstructionBoard>()!= null && GetComponentInParent<InstructionBoard>().isProcedureBoard)
            {
                e.snappedObject.GetComponent<ProcedureBlockScript>().IsTemplate = false;
                e.snappedObject.GetComponent<ProcedureBlockScript>().procedure = GetComponentInParent<InstructionBoard>().procedure;
            }
            else
            {
                e.snappedObject.GetComponent<InstructionBlock>().IsTemplate = false;
            }
        }
    }
}
