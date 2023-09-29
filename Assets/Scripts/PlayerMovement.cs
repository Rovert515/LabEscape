using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the player's movement and shifting ability
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance { get; private set; }

    public bool moving { get; private set; } // if the player is currently moving
    public bool riding { get; private set; }
    public Vector3Int gridPos { get; private set; }

    private Animator animator;
    private float moveStartTime;
    private Vector3 moveStartPos;

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        GameManager.instance.initializeOthers += Initialize;
        GameManager.instance.gameUpdate += GameUpdate;
    }
    private void OnDisable()
    {
        GameManager.instance.initializeOthers -= Initialize;
        GameManager.instance.gameUpdate -= GameUpdate;
    }
    public void Initialize()
    {
        moving = false;
        transform.position = new Vector3(LevelController.instance.width/2, LevelController.instance.width / 2);
        gridPos = LevelController.instance.grid.WorldToCell(transform.position);
        transform.position = LevelController.instance.CenterOfBlock(gridPos);
    }
    private void GameUpdate()
    {
        if (moving)
        {
            if (GameManager.instance.gameTime >= moveStartTime + GameManager.instance.settings.moveTime)
            {
                moving = false;
                animator.SetBool("moving", moving);
                transform.position = LevelController.instance.CenterOfBlock(gridPos);
                PlayerController.instance.PickUp();
            }
            else
            {
                // move towards gridPos
                transform.position = Vector3.Lerp(moveStartPos, LevelController.instance.CenterOfBlock(gridPos), (GameManager.instance.gameTime - moveStartTime)/GameManager.instance.settings.moveTime);
            }
        }
    }

    // Attempt to move in the direction dir, return successfulness
    public bool Move(Vector3Int dir)
    {
        if (LevelController.instance.InBounds(gridPos + dir) && !moving && !LevelController.instance.GetBlock(gridPos).shifting)
        {
            Block targetBlock = LevelController.instance.GetBlock(gridPos + dir);
            if (!LevelController.instance.GetBlock(gridPos).GetWall(dir) && !targetBlock.GetWall(-dir) && !targetBlock.shifting)
            {
                gridPos += dir;
                moving = true;
                animator.SetBool("moving", moving);
                moveStartTime = GameManager.instance.gameTime;
                moveStartPos = transform.position;
                return true;
            }
        }
        return false;
    }

    // Attemt to shift the level centered at the player, return successfulness
    public bool Shift(Vector3Int dir)
    {
        if (LevelController.instance.InBounds(gridPos + dir) && !moving)
        {
            if (LevelController.instance.ShiftFrom(gridPos, dir))
            {
                return true;
            }
        }
        return false;
    }

    public void StartRiding(Vector3Int shift)
    {
        if (!riding)
        {
            riding = true;
            transform.parent = LevelController.instance.GetBlock(gridPos).transform;
            gridPos += shift;
        }
    }
    public void StopRiding()
    {
        riding = false;
        transform.parent = LevelController.instance.transform;
        transform.position = LevelController.instance.CenterOfBlock(gridPos);
    }

}
