using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance { get; private set; }

    public float moveTime;

    private bool moving;
    private Vector3Int gridPos;

    private void Awake()
    {
        instance = this;

        moving = false;
    }
    private void Start()
    {
        gridPos = LevelController.instance.grid.WorldToCell(transform.position);
    }
    private void Update()
    {
        if (!moving && !LevelController.instance.shifting)
        {
            Vector3Int inputDir = Vector3Int.zero;
            if (Input.GetButtonDown("Right"))
            {
                inputDir = Vector3Int.right;
            }
            else if (Input.GetButtonDown("Left"))
            {
                inputDir = Vector3Int.left;
            }
            else if (Input.GetButtonDown("Up"))
            {
                inputDir = Vector3Int.up;
            }
            else if (Input.GetButtonDown("Down"))
            {
                inputDir = Vector3Int.down;
            }
            if (inputDir != Vector3Int.zero)
            {
                if (Input.GetButton("Shift"))
                {
                    if (LevelController.instance.InBounds(gridPos + inputDir))
                    {
                        StartCoroutine(RideWithBlock());
                        gridPos += inputDir;
                        LevelController.instance.ShiftFrom(gridPos, inputDir);
                    }
                }
                else
                {
                    Move(inputDir);
                }
            }
        }
    }
    private bool Move(Vector3Int dir)
    {
        if (LevelController.instance.InBounds(gridPos + dir))
        {
            if (LevelController.instance.GetBlock(gridPos).IsOpen(dir) && LevelController.instance.GetBlock(gridPos + dir).IsOpen(-dir))
            {
                gridPos += dir;
                transform.position = LevelController.instance.grid.GetCellCenterWorld(gridPos);
                return true;
            }
        }
        return false;
    }
    IEnumerator RideWithBlock()
    {
        transform.parent = LevelController.instance.GetBlock(gridPos).transform;
        yield return new WaitForSeconds(LevelController.instance.shiftTime);
        transform.parent = LevelController.instance.transform;
    }
}
