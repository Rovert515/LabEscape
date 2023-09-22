using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class LevelController : MonoBehaviour
{
    public static LevelController instance { get; private set; }

    public GameObject blockPrefab;
    public int levelWidth;
    public float density;
    public float keyChance;
    public float shiftTime;

    public bool shifting { get; private set; }
    public Grid grid { get; private set; }
    public Vector3 cellShift { get; private set; }
    public int topRow { get; private set; }
    public int bottomRow { get; private set; }

    private float lastShiftTime;

    private void Awake()
    {
        instance = this;

        grid = GetComponent<Grid>();
        cellShift = grid.cellSize + grid.cellGap;
        lastShiftTime = -shiftTime;
        shifting = false;
        topRow = 0;
        bottomRow = 0;
    }
    private void Update()
    {
        if (Time.time > lastShiftTime + shiftTime)
        {
            shifting = false;
        }
        else
        {
            shifting = true;
        }
        if (Camera.main.ViewportToWorldPoint(Vector3.up).y > grid.CellToWorld(new Vector3Int(0, topRow)).y)
        {
            AddRow();
        }
        if (Camera.main.ViewportToWorldPoint(Vector3.zero).y > grid.CellToWorld(new Vector3Int(0, bottomRow + 1)).y)
        {
            RemoveRow();
        }
    }
    private void AddRow()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            Instantiate(blockPrefab, grid.CellToWorld(new Vector3Int(x, topRow)), Quaternion.identity, transform);
        }
        topRow++;
    }
    private void RemoveRow()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            Block block = GetBlock(new Vector3Int(x, bottomRow));
            if (block != null)
            {
                Destroy(block.gameObject);
            }
        }
        bottomRow++;
    }
    private Block CreateBlock(Vector3Int pos)
    {
        Block block = Instantiate(blockPrefab, grid.CellToWorld(pos), Quaternion.identity, transform).GetComponent<Block>();
        return block;
    }
    public Block GetBlock(Vector3Int pos)
    {
        foreach (Transform child in transform)
        {
            Block block = child.GetComponent<Block>();
            if (block != null)
            {
                if (block.gridPos == pos && !block.fading)
                {
                    return block;
                }
            }
        }
        return null;
    }
    private List<Block> GetColumn(int n)
    {
        List<Block> column = new List<Block>();
        for (int y = bottomRow; y < topRow; y++)
        {
            Block block = GetBlock(new Vector3Int(n, y));
            if (block != null)
            {
                column.Add(block);
            }
        }
        return column;
    }
    private List<Block> GetRow(int n)
    {
        List<Block> row = new List<Block>();
        for (int x = 0; x < levelWidth; x++)
        {
            Block block = GetBlock(new Vector3Int(x, n));
            if (block != null)
            {
                row.Add(block);
            }
        }
        return row;
    }
    public bool ShiftColumn(int n, int shift)
    {
        if (!shifting)
        {
            Block addedBlock = null;
            if (shift == 1)
            {
                addedBlock = CreateBlock(new Vector3Int(n, bottomRow - 1));
            }
            else
            {
                addedBlock = CreateBlock(new Vector3Int(n, topRow));
            }
            List<Block> column = GetColumn(n);
            column.Add(addedBlock);
            foreach (Block block in column)
            {
                block.Shift(new Vector3Int(0, shift));
                if (!InBounds(block.gridPos))
                {
                    block.Fade();
                }
            }
            lastShiftTime = Time.time;
            return true;
        }
        return false;
        
    }
    public bool ShiftRow(int n, int shift)
    {
        if (!shifting)
        {
            Block addedBlock = null;
            if (shift == 1)
            {
                addedBlock = CreateBlock(new Vector3Int(-1, n));
            }
            else
            {
                addedBlock = CreateBlock(new Vector3Int(levelWidth, n));
            }
            List<Block> row = GetRow(n);
            row.Add(addedBlock);
            foreach (Block block in row)
            {
                block.Shift(new Vector3Int(shift, 0));
                if (!InBounds(block.gridPos))
                {
                    block.Fade();
                }
            }
            lastShiftTime = Time.time;
            return true;
        }
        return false;
    }
    public bool ShiftFrom(Vector3Int origin, Vector3Int dir)
    {
        if (dir.x == 0)
        {
            return ShiftColumn(origin.x, dir.y);
        }
        else if (dir.y == 0)
        {
            return ShiftRow(origin.y, dir.x);
        }
        return false;
    }
    public bool InBounds(Vector3Int pos)
    {
        return (pos.x >= 0) && (pos.y >= bottomRow) && (pos.x < levelWidth) && (pos.y < topRow);
    }
}
