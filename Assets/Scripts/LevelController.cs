using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class LevelController : MonoBehaviour
{
    public GameObject blockPrefab;
    private Grid grid;
    public int levelWidth;
    public int levelHeight;
    public float density;
    public float keyChance;
    private Vector3 cellShift;
    private float lastShiftTime;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        cellShift = grid.cellSize + grid.cellGap;
        lastShiftTime = Time.time;
    }
    private void Start()
    {
        MakeSquare();
    }
    private void Update()
    {
        if (Time.time > lastShiftTime + 0.3f)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ShiftColumn(2, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ShiftColumn(2, -1);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ShiftRow(3, 1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ShiftRow(3, -1);
            }
        }
    }
    private void MakeSquare()
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
    private Block GetBlock(Vector3Int pos)
    {
        foreach (Transform child in transform)
        {
            Block block = child.GetComponent<Block>();
            if (block)
            {
                if (grid.WorldToCell(block.desiredPos) == pos)
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
    private void ShiftColumn(int n, int shift)
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
            if (block.GetGridPos().y < 0 || block.GetGridPos().y >= levelHeight)
            {
                block.Fade();
            }
        }
        lastShiftTime = Time.time;
    }
    private void ShiftRow(int n, int shift)
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
            if (block.GetGridPos().x < 0 || block.GetGridPos().x >= levelWidth)
            {
                block.Fade();
            }
        }
        lastShiftTime = Time.time;
    }
}
