using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

/**
Game manager class that, well manages the game:
spawns the player,
spawns the level,
*/
public class GameManager : MonoBehaviour
{
    public InstructionBoard ib;
    public InstructionBoard pb;
    public static GameManager GM;
    public AudioSource asrc;
    public AudioClip successClip;
    public AudioClip failureClip;
    public GameObject outsideWalls;
    public GameObject walls;
    private GameObject[,] levelFloor;
    public GameObject tile;
    public GameObject forwardSlot;
    public GameObject leftSlot;
    public GameObject rightSlot;
    public GameObject jumpSlot;
    public GameObject startTile;
    public GameObject tick;
    public GameObject cross;
    public GameObject infoText;
    public GameObject table;
    public GameObject level1, level2, level3, level4, level5;
    public GameObject robot;
    public GameObject instructionBoard;
    public GameObject procedureDesk;
    public GameObject robotPrefab;
    public Material startMat;
    public Material endMat;
    public Material defaultMat;
    public int level = 2;
    public LinkedList<RobotScript.Command> commands;
    private bool robotExists;
    private bool cameFromLevel3;
    private bool cameFromLevel4;
    
    
    private int i, j; // variables to help with looping of SetUpTile to keep track of which tile we are up to outside of the method

    /**
     * Game manager set up as singleton: to access the game manager from anywhere just access GameManager.GM
     */
    void Start()
    {
        if (GM == null)
        {
            GM = this;
        }
        else
        {
            Destroy(this);
        }
        commands = new LinkedList<RobotScript.Command>();  // initialize the list to store the command queue

        SetUpLevelFloor();
    }
    

    /**
     * 2d array holds the game objects for the tiles
     * InvokeRepeating - method SetUpTile is called every 0.05 seconds for a different tile to create the rising tiles effect
     */
    void SetUpLevelFloor()
    {
        levelFloor = new GameObject[7, 7];
        InvokeRepeating("SetUpTile", 0.0f, 0.05f);
    }

    /**
     * creates a tile and spawns it in the scene. the position is set according to the tile's index, which is kept track of
     * by i and j outside of the method. -5 on the y axis is because the tiles are spawned below the level and then they rise up
     * for the cool effect
     */
    void SetUpTile()
    {
        levelFloor[i, j] = Instantiate(tile);
        levelFloor[i,j].transform.position = new Vector3((float)i/2, -2, (float)j/2);
        if (j == 6)
        {
            i++;
            if (i == 7)
            {
                i = 0;
                j = 0;
                CancelInvoke("SetUpTile");  // last tile was created, stop creating new tiles
                Invoke("SetUpLevel", 1);  // last tile was created, stop creating new tiles
                Invoke("SetUpRobot", 4);    // wait for a few seconds for all tiles to rise to the level floor and spawn the robot
                return;
            }
            j = 0;
        }
        else
        {
            j++;
        }
    }

    /**
     * spawning the robot
     */
    public void SetUpRobot()
    {
        if (robotExists)
            Destroy(robot);
        robot = Instantiate(robotPrefab, table.transform.position, Quaternion.identity); // spawn the robot
        robot.transform.eulerAngles = new Vector3(0, 180, 0);
        robot.GetComponent<Collider>().enabled = true; //enable collisions
        robot.transform.Translate(new Vector3(0,1)); // move it a little up so it drops down
        robotExists = true;
    }


    
    /**
    * set up the level walls
    */
    public void SetUpLevel()
    {
        leftSlot.GetComponent<VRTK_SnapDropZone>().cloneNewOnUnsnap = false;
        rightSlot.GetComponent<VRTK_SnapDropZone>().cloneNewOnUnsnap = false;
        forwardSlot.GetComponent<VRTK_SnapDropZone>().cloneNewOnUnsnap = false;
        jumpSlot.GetComponent<VRTK_SnapDropZone>().cloneNewOnUnsnap = false;
        Destroy(leftSlot.GetComponent<VRTK_SnapDropZone>().GetCurrentSnappedObject());
        Destroy(forwardSlot.GetComponent<VRTK_SnapDropZone>().GetCurrentSnappedObject());
        Destroy(rightSlot.GetComponent<VRTK_SnapDropZone>().GetCurrentSnappedObject());
        Destroy(jumpSlot.GetComponent<VRTK_SnapDropZone>().GetCurrentSnappedObject());
        forwardSlot.SetActive(true);
        leftSlot.SetActive(true);
        rightSlot.SetActive(true);
        jumpSlot.SetActive(true);

        //level specific set up
        if (cameFromLevel3)
        {
            for (int i = 0; i < 3; i++)
            {
                levelFloor[2 + i, 4].GetComponent<TileScript>().positionTarget.y -= 0.15f;
                levelFloor[2 + i, 4].GetComponent<TileScript>().settingup = true;
            }
            levelFloor[4, 5].GetComponent<TileScript>().positionTarget.y -= 0.3f;
            levelFloor[4, 5].GetComponent<TileScript>().settingup = true;
            levelFloor[4, 6].GetComponent<TileScript>().positionTarget.y -= 0.3f;
            levelFloor[4, 6].GetComponent<TileScript>().settingup = true;
            cameFromLevel3 = false;
        }
        else if (cameFromLevel4)
        {
            for (int i = 0; i < 4; i++)
            {
                levelFloor[2 + i, 5].GetComponent<TileScript>().positionTarget.y -= 0.15f;
                levelFloor[2 + i, 5].GetComponent<TileScript>().settingup = true;
                levelFloor[2 + i, 3].GetComponent<TileScript>().positionTarget.y -= 0.15f;
                levelFloor[2 + i, 3].GetComponent<TileScript>().settingup = true;
                levelFloor[2 + i, 2].GetComponent<TileScript>().positionTarget.y -= 0.3f;
                levelFloor[2 + i, 2].GetComponent<TileScript>().settingup = true;
            }
            cameFromLevel4 = false;
        }

        // walls set up
        if(walls!= null)
            walls.GetComponent<WallScript>().unsettingup = true;
        Renderer rend = levelFloor[3, 3].GetComponentInChildren<Renderer>();
        var mats = rend.materials;
        foreach (GameObject tile in levelFloor)
        {
            rend = tile.GetComponentInChildren<Renderer>();
            mats = rend.materials;
            mats[0] = defaultMat; 
            rend.materials = mats;
        }
        // set up environment for each level
        switch (level)
        {
            case 1:
                procedureDesk.SetActive(false);
                leftSlot.SetActive(false);
                rightSlot.SetActive(false);
                jumpSlot.SetActive(false);
                infoText.GetComponent<Text>().text =
                    "Welcome to LogiBot!\n\n You need to guide your robot through the maze, from the yellow square to the green square.\n\n" +
                    "Create your program by dragging the instruction blocks on the left into the program tray in front of you.\n\n" +
                    "Hold the trigger to grab highlighted objects. Use the touchpad click to clear the instruction board,\n" +
                    "quick-add instructions or quickstart the robot. Use the touchpad scroll to move held instructions forwards and backwards.\n\n" +
                    "The forward block instructs your robot to move forward one square in the direction it is facing.\n\n" +
                    "When you are done, grab the robot and drop it on the maze to set if off!";
                startTile = levelFloor[3, 6];
                walls = Instantiate(level1, new Vector3(levelFloor[3, 3].gameObject.transform.position.x, -5,levelFloor[3, 3].gameObject.transform.position.z), Quaternion.identity);
                walls.GetComponent<WallScript>().positionTarget = new Vector3(1.75f, 0.5f, 0.2f);                
                startTile = levelFloor[3, 6];
                rend = levelFloor[3, 3].GetComponentInChildren<Renderer>();
                levelFloor[3, 3].GetComponent<TileScript>().isEnd = true;
                mats = rend.materials;
                mats[0] = endMat; 
                rend.materials = mats;
                rend = levelFloor[3, 6].GetComponentInChildren<Renderer>();
                mats = rend.materials;
                mats[0] = startMat; 
                rend.materials = mats;
                break;    
            case 2:
                procedureDesk.SetActive(false);
                rightSlot.SetActive(false);
                jumpSlot.SetActive(false);
                infoText.GetComponent<Text>().text = "You need to use an additional block to solve this next maze.\n" +
                                                     "\nThe rotate block instructs your robot to rotate 90 degrees. " +
                                                     "This particular rotate block rotates your robot anti clockwise.\n";
                startTile = levelFloor[3, 6];
                walls = Instantiate(level2, new Vector3(levelFloor[3, 3].gameObject.transform.position.x, -5,levelFloor[3, 3].gameObject.transform.position.z), Quaternion.identity);
                walls.transform.eulerAngles = new Vector3(-90,0);
                walls.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                walls.GetComponent<WallScript>().positionTarget = new Vector3(1.75f, 0.5f, 0.2f);
                rend = levelFloor[3, 6].GetComponentInChildren<Renderer>();
                mats = rend.materials;
                mats[0] = startMat; 
                rend.materials = mats;
                levelFloor[4, 6].GetComponent<TileScript>().isEnd = true;
                rend = levelFloor[4, 6].GetComponentInChildren<Renderer>();
                mats = rend.materials;
                mats[0] = endMat; 
                rend.materials = mats;
                break;
            case 3:
                procedureDesk.SetActive(false);
                infoText.GetComponent<Text>().text = "The next block tells your robot to move forward and jump at the same time.\n" +
                                                     "\nUse this block and the others you have previously used to solve this maze.";
                walls = Instantiate(level3, new Vector3(levelFloor[3, 3].gameObject.transform.position.x, -5,levelFloor[3, 3].gameObject.transform.position.z), Quaternion.identity);
                walls.transform.eulerAngles = new Vector3(-90,0);
                walls.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                walls.GetComponent<WallScript>().positionTarget = new Vector3(1.25f, 0.5f, 0.2f);
                startTile = levelFloor[2, 6];
                rend = levelFloor[2, 6].GetComponentInChildren<Renderer>();
                mats = rend.materials;
                mats[0] = startMat; 
                rend.materials = mats;
                rend = levelFloor[4, 6].GetComponentInChildren<Renderer>();
                levelFloor[4, 6].GetComponent<TileScript>().isEnd = true;
                mats = rend.materials;
                mats[0] = endMat; 
                rend.materials = mats;
                for (int i = 0; i < 3; i++)
                {
                    levelFloor[2 + i, 4].GetComponent<TileScript>().positionTarget.y += 0.15f;
                    levelFloor[2 + i, 4].GetComponent<TileScript>().settingup = true;
                }
                levelFloor[4, 5].GetComponent<TileScript>().positionTarget.y += 0.3f;
                levelFloor[4, 5].GetComponent<TileScript>().settingup = true;
                levelFloor[4, 6].GetComponent<TileScript>().positionTarget.y += 0.3f;
                levelFloor[4, 6].GetComponent<TileScript>().settingup = true;
                cameFromLevel3 = true;
                break;
            case 4:
                procedureDesk.SetActive(false);
                infoText.GetComponent<Text>().text = "Practice the blocks you have learned previously." +
                                                     "\n\nThere are multiple ways to solve this maze, don’t be afraid to be creative!\n";
                walls = Instantiate(level4, new Vector3(levelFloor[3, 3].gameObject.transform.position.x, -5,levelFloor[3, 3].gameObject.transform.position.z), Quaternion.identity);
                walls.transform.eulerAngles = new Vector3(-90,0);
                walls.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                walls.GetComponent<WallScript>().positionTarget = new Vector3(1.75f, 0.5f, 0.2f);
                startTile = levelFloor[5, 6];
                rend = levelFloor[5, 6].GetComponentInChildren<Renderer>();
                mats = rend.materials;
                mats[0] = startMat; 
                rend.materials = mats;
                levelFloor[2, 2].GetComponent<TileScript>().isEnd = true;
                rend = levelFloor[2, 2].GetComponentInChildren<Renderer>();
                mats = rend.materials;
                mats[0] = endMat; 
                rend.materials = mats;
                for (int i = 0; i < 4; i++)
                {
                    levelFloor[2 + i, 5].GetComponent<TileScript>().positionTarget.y += 0.15f;
                    levelFloor[2 + i, 5].GetComponent<TileScript>().settingup = true;
                    levelFloor[2 + i, 3].GetComponent<TileScript>().positionTarget.y += 0.15f;
                    levelFloor[2 + i, 3].GetComponent<TileScript>().settingup = true;
                    levelFloor[2 + i, 2].GetComponent<TileScript>().positionTarget.y += 0.3f;
                    levelFloor[2 + i, 2].GetComponent<TileScript>().settingup = true;
                }
                cameFromLevel4 = true;
                break;
            case 5:
                infoText.GetComponent<Text>().text = "For this maze you will need more space than the 12 slots available in your main program.\n\n" +
                                                     "This is where you can create a procedure block to issue a sequence of blocks multiple times.\n\n" +
                                                     "Use the second instruction board on your right to create a procedure that contains several commands.\n" +
                                                     "Use the procedure block just like a normal instruction block.";
                procedureDesk.SetActive(true);
                startTile = levelFloor[1, 6];
                walls = Instantiate(level5, new Vector3(levelFloor[3, 3].gameObject.transform.position.x, -5,levelFloor[3, 3].gameObject.transform.position.z), Quaternion.identity);
                walls.transform.eulerAngles = new Vector3(-90,0);
                walls.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                walls.GetComponent<WallScript>().positionTarget = new Vector3(1.75f, 0.5f, 0.2f);
                rend = levelFloor[5, 6].GetComponentInChildren<Renderer>();
                mats = rend.materials;
                mats[0] = endMat;
                rend.materials = mats;
                levelFloor[5, 6].GetComponent<TileScript>().isEnd = true;
                rend = levelFloor[1, 6].GetComponentInChildren<Renderer>();
                mats = rend.materials;
                mats[0] = startMat;
                rend.materials = mats;
                break; 
        }
        leftSlot.GetComponent<VRTK_SnapDropZone>().cloneNewOnUnsnap = true;
        rightSlot.GetComponent<VRTK_SnapDropZone>().cloneNewOnUnsnap = true;
        forwardSlot.GetComponent<VRTK_SnapDropZone>().cloneNewOnUnsnap = true;
        jumpSlot.GetComponent<VRTK_SnapDropZone>().cloneNewOnUnsnap = true;
        TileScript.firstCollision = true;
        walls.GetComponent<WallScript>().settingup = true;
        foreach (var collider in  walls.GetComponents<Collider>())
        {
            collider.enabled = false;
        }
        foreach (var collider in outsideWalls.GetComponents<Collider>())
        {
            collider.enabled = false;
        }
    }

    public void LevelSuccess()
    {
        ClearInstructions();
        tick.SetActive(true);
        asrc.clip = successClip;
        asrc.Play();
    }
    
    public void LevelFail()
    {
        Invoke("RefreshCommands", 4.5f);
        cross.SetActive(true);
        asrc.clip = failureClip;
        asrc.Play();
    }

    private void RefreshCommands()
    {
        ib.SetCommands(null, new SnapDropZoneEventArgs());
    }

    public void HideCrossTick()
    {
        cross.SetActive(false);
        tick.SetActive(false);
    }
    public void ClearInstructions()
    {
        foreach(Transform child in instructionBoard.transform)
        {
            var obj = child.GetComponent<VRTK_SnapDropZone>();
            if (obj != null && obj.GetCurrentSnappedObject() != null)
            {
                Destroy(obj.GetCurrentSnappedObject());
            }
        }
    }
}
