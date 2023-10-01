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
    private Vector3Int moveDir;

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
        transform.position = LevelController.instance.transform.position + new Vector3(1, 1) * LevelController.instance.width / 2;
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
                Vector3 newPos = LevelController.instance.CenterOfBlock(gridPos) - Vector3.Scale((Vector3)moveDir, LevelController.instance.cellShift) * (1-(GameManager.instance.gameTime - moveStartTime)/GameManager.instance.settings.moveTime);
                newPos.x = (newPos.x + LevelController.instance.width) % LevelController.instance.width;
                transform.position = newPos;
            }
        }
        if (riding)
        {
            Vector3 newPos = transform.position;
            newPos.x = (newPos.x + LevelController.instance.width) % LevelController.instance.width;
            transform.position = newPos;
        }
        
    }

    // Attempt to move in the direction dir, return successfulness
    public bool Move(Vector3Int dir)
    {
        Vector3Int targetPos = gridPos + dir;
        targetPos.x = (targetPos.x + LevelController.instance.levelWidth) % LevelController.instance.levelWidth;
        if (LevelController.instance.InBounds(targetPos) && !moving && !LevelController.instance.GetBlock(gridPos).shifting)
        {
            Block targetBlock = LevelController.instance.GetBlock(targetPos);
            if (!LevelController.instance.GetBlock(gridPos).GetWall(dir) && !targetBlock.GetWall(-dir) && !targetBlock.shifting)
            {
                moving = true;
                animator.SetBool("moving", moving);
                moveStartTime = GameManager.instance.gameTime;
                moveDir = dir;
                gridPos = targetPos;
                return true;
            }
        }
        return false;
    }

    // Attemt to shift the level centered at the player, return successfulness
    public bool Shift(Vector3Int dir)
    {
        Vector3Int targetPos = gridPos + dir;
        targetPos.x = (targetPos.x + LevelController.instance.levelWidth) % LevelController.instance.levelWidth;
        if (LevelController.instance.InBounds(targetPos) && !moving)
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
            gridPos = new Vector3Int((gridPos.x + LevelController.instance.levelWidth) % LevelController.instance.levelWidth, gridPos.y);
        }
    }
    public void StopRiding()
    {
        riding = false;
        transform.parent = LevelController.instance.transform;
        transform.position = LevelController.instance.CenterOfBlock(gridPos);
    }

}
