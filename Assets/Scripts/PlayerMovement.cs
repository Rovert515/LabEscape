using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the player's movement and shifting ability
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance { get; private set; }

    public bool moving { get; private set; } // if the player is currently moving
    public Vector3Int gridPos { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    public void Initialize()
    {
        moving = false;
        transform.position = new Vector3(LevelController.instance.width/2, Camera.main.orthographicSize);
        gridPos = LevelController.instance.grid.WorldToCell(transform.position);
        transform.position = LevelController.instance.CenterOfBlock(gridPos);

        // Make sure the player can exit the starting block
        LevelController.instance.GetBlock(gridPos).SetWall(Vector3Int.up, false);
    }

    // Attempt to move in the direction dir, return successfulness
    public bool Move(Vector3Int dir)
    {
        if (LevelController.instance.InBounds(gridPos + dir) && !moving && !LevelController.instance.shifting)
        {
            if (!LevelController.instance.GetBlock(gridPos).GetWall(dir) && !LevelController.instance.GetBlock(gridPos + dir).GetWall(-dir))
            {
                gridPos += dir;
                StartCoroutine(MoveRoutine());
                return true;
            }
        }
        return false;
    }

    // Move transform.position to gridPos at a constant speed in moveTime seconds
    IEnumerator MoveRoutine()
    {
        moving = true;
        float startTime = Time.time;
        Vector3 targetPos = LevelController.instance.CenterOfBlock(gridPos);
        Vector3 startPos = transform.position;
        while (Time.time < startTime + GameManager.instance.settings.moveTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, (Time.time - startTime) / GameManager.instance.settings.moveTime);
            yield return null;
        }
        transform.position = targetPos;
        PlayerController.instance.ConsumeMana();
        moving = false;
    }

    // Attemt to shift the level centered at the player, return successfulness
    public bool Shift(Vector3Int dir)
    {
        if (LevelController.instance.InBounds(gridPos + dir) && !moving)
        {
            if (LevelController.instance.ShiftFrom(gridPos, dir))
            {
                StartCoroutine(RideWithBlock());
                gridPos += dir;
                return true;
            }
        }
        return false;
    }

    // Temporarily set the parent of the player to the block they are standing in so that they shift along with the block
    IEnumerator RideWithBlock()
    {
        transform.parent = LevelController.instance.GetBlock(gridPos).transform;
        yield return new WaitForSeconds(GameManager.instance.settings.shiftTime);
        transform.parent = LevelController.instance.transform;
        transform.position = LevelController.instance.CenterOfBlock(gridPos);
    }
    
}
