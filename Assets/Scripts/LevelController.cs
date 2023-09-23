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
    public float manaChance;
    public float shiftTime;

    // Whether or not there are any blocks currently shifting
    public bool shifting { get; private set; }
    public Grid grid { get; private set; }
    // Horizontal and vertical distance between the centers of cells
    public Vector3 cellShift { get; private set; }
    // Row above the current highest row (exclusive)
    public int topRow { get; private set; }
    // The current lowest row (inclusive)
    public int bottomRow { get; private set; }

    private void Awake()
    {
        instance = this;

        grid = GetComponent<Grid>();
        cellShift = grid.cellSize + grid.cellGap;
        shifting = false;
        topRow = 0;
        bottomRow = 0;
    }
    private void Update()
    {
        // Add and remove rows when needed based on carmera position
        while (Camera.main.ViewportToWorldPoint(Vector3.up).y > grid.CellToWorld(new Vector3Int(0, topRow)).y)
        {
            AddRow();
        }
        while (Camera.main.ViewportToWorldPoint(Vector3.zero).y > grid.CellToWorld(new Vector3Int(0, bottomRow + 1)).y)
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
        UIManager.instance.UpdateUI();
    }
    private Block CreateBlock(Vector3Int gridPos)
    {
        if (GetBlock(gridPos) == null)
        {
            Block block = Instantiate(blockPrefab, grid.CellToWorld(gridPos), Quaternion.identity, transform).GetComponent<Block>();
            return block;
        }
        Debug.Log("Failed to create block");
        return null;
        
    }
    public Block GetBlock(Vector3Int gridPos)
    {
        foreach (Transform child in transform)
        {
            Block block = child.GetComponent<Block>();
            if (block != null)
            {
                if (block.gridPos == gridPos && !block.fading)
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
    // shift argument should be either 1 or -1
    public bool ShiftColumn(int n, int shift)
    {
        if (!shifting)
        {
            StartCoroutine(ShiftRoutine());
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
            return true;
        }
        return false;
        
    }
    // shift argument should be either 1 or -1
    public bool ShiftRow(int n, int shift)
    {
        if (!shifting)
        {
            StartCoroutine(ShiftRoutine());
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
    public bool InBounds(Vector3Int gridPos)
    {
        return (gridPos.x >= 0) && (gridPos.y >= bottomRow) && (gridPos.x < levelWidth) && (gridPos.y < topRow);
    }
    public Vector3 CenterOfBlock(Vector3Int gridPos)
    {
        return grid.CellToWorld(gridPos) + grid.cellSize/2;
    }
    // Updates shifting variable
    IEnumerator ShiftRoutine()
    {
        shifting = true;
        yield return new WaitForSeconds(shiftTime);
        shifting = false;
    }
}
