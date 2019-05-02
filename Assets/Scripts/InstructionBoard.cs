using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class InstructionBoard : MonoBehaviour
{

    public bool isProcedureBoard;
    
    [Tooltip("The prefab to instantiate for the slots on board")]
    public GameObject instructionSlotPrefab;

    public RobotScript.Procedure procedure = new RobotScript.Procedure();
    
    [SerializeField]
    [Tooltip("Space between each slot")]
    private float offset = 0.12f;

    [SerializeField]
    [Tooltip("Size of the vertical axis of slots")]
    private int verticalAmount = 3;

    [SerializeField]
    [Tooltip("Size of the horizontal axis of slots")]
    private int horizontalAmount = 4;

    private GameObject[,] slots;

    //zone that is overriding a snap to another zone
    private VRTK_SnapDropZone overridingZone;

	// Use this for initialization
	void Start () {
        if (!instructionSlotPrefab)
            throw new System.Exception("You need to provide a slot prefab to instantiate!");
	    slots = new GameObject[verticalAmount,horizontalAmount];
        //create the new slots in a grid layout
        for (int i = 0; i < verticalAmount; i++)
        {
            for (int j = 0; j < horizontalAmount; j++)
            {
                slots[i,j] = Instantiate(instructionSlotPrefab, this.transform);
                slots[i,j].transform.position = new Vector3(transform.position.x - (offset*j), transform.position.y, transform.position.z + (offset*i));
                slots[i,j].name = "slot " + i + "-" + j;
                VRTK_SnapDropZone zone = slots[i,j].GetComponent<VRTK_SnapDropZone>();
                zone.ObjectSnappedToDropZone += OverrideSnap;
                zone.ObjectSnappedToDropZone += SetCommands;
                zone.ObjectUnsnappedFromDropZone += SetCommands;
                zone.validObjectListPolicy = GetComponentInChildren<VRTK_PolicyList>();
                slots[i, j].GetComponent<InstructionBlockSlot>().parentBoard = this;
            }   
        }
	}

    //when an object is snapped to a point on the board, move it to the next empty slot
    private void OverrideSnap(object sender, SnapDropZoneEventArgs e)
    {
        VRTK_SnapDropZone currentZone = (VRTK_SnapDropZone) sender;
        if (overridingZone != currentZone)
        {
            VRTK_SnapDropZone newZone = GetNextEmptySlot(currentZone);
            overridingZone = newZone;
            Debug.Log("Overriding!");
            if (newZone == currentZone)
                return;
            currentZone.ForceUnsnap();
            newZone.ForceSnap(e.snappedObject);
        }
        else
            overridingZone = null;
    }

    /// <summary>
    /// Return the next empty snap drop zone on the board
    /// </summary>
    /// <returns>The next empty slot to snap to, null if there is none</returns>
    public VRTK_SnapDropZone GetNextEmptySlot()
    {
        return GetNextEmptySlot(null);
    }

    /// <summary>
    ///return the next empty snap drop zone on the board
    /// </summary>
    /// <param name="selected">The zone that was initially selected for snapping. If there was no inital zone, pass in null</param>
    /// <returns>The next empty slot to snap to, null if there is none</returns>
    public VRTK_SnapDropZone GetNextEmptySlot(VRTK_SnapDropZone selected)
    {
        foreach (Transform child in transform)
        {
            VRTK_SnapDropZone snapDropZone;
            if (snapDropZone = child.GetComponent<VRTK_SnapDropZone>())
            {
                if (!snapDropZone.GetCurrentSnappedObject() || snapDropZone == selected)
                    return snapDropZone;
            }
        }
        return null;
    }

    /// <summary>
    /// Automatically snap the passed in block to a free slot on this board
    /// </summary>
    /// <param name="block">The block instance to snap to a slot</param>
    public void AutoSnapToBoard(InstructionBlock block)
    {
        VRTK_SnapDropZone zone = GetNextEmptySlot();
        if (!zone)
        {
            //Debug.Log("No Free Slot!");
            return;
        }
        GameObject blockObject = block.gameObject;
        GameObject zoneObject = zone.gameObject;
        blockObject.transform.position = zoneObject.transform.position;
        zone.ForceSnap(blockObject);
    }

    public void AutoSnapToBoard(ProcedureBlockScript block)
    {
        VRTK_SnapDropZone zone = GetNextEmptySlot();
        if (!zone)
        {
            //Debug.Log("No Free Slot!");
            return;
        }
        GameObject blockObject = block.gameObject;
        GameObject zoneObject = zone.gameObject;
        blockObject.transform.position = zoneObject.transform.position;
        zone.ForceSnap(blockObject);
    }

    /// <summary>
    /// Evaluate a linkedlist containing all of the commands contained within this board
    /// </summary>
    public void SetCommands(object sender, SnapDropZoneEventArgs snapDropZoneEventArgs)
    {
        var rs = GameManager.GM.robot.GetComponent<RobotScript>();

        if (isProcedureBoard)
        {        
            procedure = new RobotScript.Procedure();
        }
        else
        {
            rs.commands = new LinkedList<RobotScript.Command>();
        }
        foreach (var slot in slots)
        {
            var slotScript = slot.GetComponent<InstructionBlockSlot>();
            if (isProcedureBoard && slotScript != null)
            {
                if (slotScript.Command != RobotScript.Command.None)
                    procedure.AddCommand(slotScript.Command);
            }
            else if (!isProcedureBoard && slotScript != null)
            {
                var snappedObject = slotScript.snapDrop.GetCurrentSnappedObject();
                if (snappedObject)
                {
                    var instructionBlockScript = snappedObject.GetComponent<InstructionBlock>();
                    var procedureBlockScript = snappedObject.GetComponent<ProcedureBlockScript>();
                    if (instructionBlockScript && slotScript.Command != RobotScript.Command.None)
                        rs.AddCommand(instructionBlockScript.robotCommand);
                    else if (procedureBlockScript)
                        rs.AddCommand(procedureBlockScript.procedure);
                }
            }
        }
    }

}
