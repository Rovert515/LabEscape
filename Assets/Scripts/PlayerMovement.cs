using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance { get; private set; }

    public float moveTime;

    public bool moving { get; private set; }
    public Vector3Int gridPos { get; private set; }

    private void Awake()
    {
        instance = this;

        moving = false;
    }
    private void Start()
    {
        gridPos = LevelController.instance.grid.WorldToCell(transform.position);
    }
    
    public bool Move(Vector3Int dir)
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
    public bool Shift(Vector3Int dir)
    {
        if (LevelController.instance.InBounds(gridPos + dir))
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
    IEnumerator RideWithBlock()
    {
        transform.parent = LevelController.instance.GetBlock(gridPos).transform;
        yield return new WaitForSeconds(LevelController.instance.shiftTime);
        transform.parent = LevelController.instance.transform;
        transform.position = LevelController.instance.grid.GetCellCenterWorld(gridPos);
    }
}
