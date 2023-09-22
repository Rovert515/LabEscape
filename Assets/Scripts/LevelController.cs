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
    public int levelHeight;
    public float density;
    public float keyChance;
    public float shiftTime;

    public bool shifting { get; private set; }
    public Grid grid { get; private set; }
    public Vector3 cellShift { get; private set; }

    private float lastShiftTime;

    private void Awake()
    {
        instance = this;

        grid = GetComponent<Grid>();
        cellShift = grid.cellSize + grid.cellGap;
        lastShiftTime = -shiftTime;
        shifting = false;
    }
    private void Start()
    {
        BuildInitialLevel();
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
    }
    private void BuildInitialLevel()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelHeight; y++)
            {
                Instantiate(blockPrefab, grid.CellToWorld(new Vector3Int(x, y)), Quaternion.identity, transform);
            }
        }
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
                if (block.gridPos == pos)
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
        for (int y = 0; y < levelHeight; y++)
        {
            column.Add(GetBlock(new Vector3Int(n, y)));
        }
        return column;
    }
    private List<Block> GetRow(int n)
    {
        List<Block> row = new List<Block>();
        for (int x = 0; x < levelWidth; x++)
        {
            row.Add(GetBlock(new Vector3Int(x, n)));
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
                addedBlock = CreateBlock(new Vector3Int(n, -1));
            }
            else
            {
                addedBlock = CreateBlock(new Vector3Int(n, levelHeight));
            }
            List<Block> column = GetColumn(n);
            column.Add(addedBlock);
            foreach (Block block in column)
            {
                block.Shift(new Vector3Int(0, shift));
                if (block.gridPos.y < 0 || block.gridPos.y >= levelHeight)
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
        return (pos.x >= 0) && (pos.y >= 0) && (pos.x < levelWidth) && (pos.y < levelHeight);
    }
}
