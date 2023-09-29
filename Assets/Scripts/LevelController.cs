using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

// Manages all of the blocks in the level and directs their creation, movement, and destruction
public class LevelController : MonoBehaviour
{
    public static LevelController instance { get; private set; }

    public GameObject blockPrefab;
    public float manaChance;
    
    public bool shifting { get; private set; } // Whether or not there are any blocks currently shifting
    public Grid grid { get; private set; }
    public Vector3 cellShift { get; private set; } // Horizontal and vertical distance between the centers of cells
    public int topRow { get; private set; } // Row above the current highest row (exclusive)
    public int bottomRow { get; private set; } // The current lowest row (inclusive)
    public float width { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        GameManager.instance.initializeLevel += Initialize;
        GameManager.instance.gameUpdate += GameUpdate;
    }
    private void OnDisable()
    {
        GameManager.instance.initializeLevel -= Initialize;
        GameManager.instance.gameUpdate -= GameUpdate;
    }
    public void Initialize()
    {
        transform.localPosition = Vector3.zero;
        grid = GetComponent<Grid>();
        cellShift = grid.cellSize + grid.cellGap;
        width = cellShift.x * GameManager.instance.settings.levelWidth;
        shifting = false;
        topRow = 0;
        bottomRow = 0;
        UpdateRows();
    }
    private void GameUpdate()
    {
        UpdateRows();
    }

    // Get, create, and destroy blocks
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
        Debug.LogWarning("Failed to find block at " + gridPos, transform);
        return null;
    }
    private Block CreateBlock(Vector3Int gridPos)
    {
        Block block = Instantiate(blockPrefab, grid.CellToWorld(gridPos), Quaternion.identity, transform).GetComponent<Block>();
        return block;
    }
    private bool DestroyBlock(Vector3Int gridPos)
    {
        Block block = GetBlock(gridPos);
        if (block != null)
        {
            Destroy(block.gameObject);
            return true;
        }
        Debug.LogWarning("Attempted to destroy nonexistent block at " + gridPos, transform);
        return false;
    }

    // Add/remove row from top/bottom of level, update topRow/bottomRow
    private void AddRow()
    {
        for (int x = 0; x < GameManager.instance.settings.levelWidth; x++)
        {
            CreateBlock(new Vector3Int(x, topRow));
        }
        topRow++;
    }
    private void RemoveRow()
    {
        for (int x = 0; x < GameManager.instance.settings.levelWidth; x++)
        {
            DestroyBlock(new Vector3Int(x, bottomRow));
        }
        bottomRow++;
        UIManager.instance.UpdateUI();
    }

    // Add and remove rows if needed based on carmera position
    public void UpdateRows()
    {
        while (Camera.main.ViewportToWorldPoint(Vector3.up).y > grid.CellToWorld(new Vector3Int(0, topRow)).y)
        {
            AddRow();
        }
        while (Camera.main.ViewportToWorldPoint(Vector3.zero).y > grid.CellToWorld(new Vector3Int(0, bottomRow + 1)).y)
        {
            RemoveRow();
        }
    }

    // Get a list of all of the blocks in the nth column/row
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
        for (int x = 0; x < GameManager.instance.settings.levelWidth; x++)
        {
            Block block = GetBlock(new Vector3Int(x, n));
            if (block != null)
            {
                row.Add(block);
            }
        }
        return row;
    }

    // Shifts the nth column/row by shift, which should be either 1 or -1
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
            else if (shift == -1)
            {
                addedBlock = CreateBlock(new Vector3Int(n, topRow));
            }
            else
            {
                Debug.LogWarning("ShiftColumn() called with invalid shift value", transform);
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
            else if (shift == -1)
            {
                addedBlock = CreateBlock(new Vector3Int(GameManager.instance.settings.levelWidth, n));
            }
            else
            {
                Debug.Log("ShiftColumn() called with invalid shift value");
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

    // Shifts the column/row that origin is in the direction dir
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

    // Updates shifting variable
    IEnumerator ShiftRoutine()
    {
        shifting = true;
        yield return new WaitForSeconds(GameManager.instance.settings.shiftTime);
        shifting = false;
    }

    // Whether or not a gridPos is inside the bounds of the current level
    public bool InBounds(Vector3Int gridPos)
    {
        return (gridPos.x >= 0) && (gridPos.y >= bottomRow) && (gridPos.x < GameManager.instance.settings.levelWidth) && (gridPos.y < topRow);
    }

    // The center of the block at gridPos in world space
    public Vector3 CenterOfBlock(Vector3Int gridPos)
    {
        return grid.CellToWorld(gridPos) + grid.cellSize/2;
    }
}
