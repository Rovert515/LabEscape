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

    private TilemapRenderer tilemapRenderer;

    public GameObject blockPrefab;
    public int levelWidth;
    
    public Grid grid { get; private set; }
    public Vector3 cellShift { get; private set; } // Horizontal and vertical distance between the centers of cells
    public int topRow { get; private set; } // Row above the current highest row (exclusive)
    public int bottomRow { get; private set; } // The current lowest row (inclusive)
    public float width { get; private set; }

    private void Awake()
    {
        instance = this;
        tilemapRenderer = GetComponentInChildren<TilemapRenderer>();
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
        //transform.localPosition = Vector3.zero;
        grid = GetComponent<Grid>();
        cellShift = grid.cellSize + grid.cellGap;
        width = cellShift.x * levelWidth;
        topRow = 0;
        bottomRow = 0;
        ClearLevel();
        UpdateLevelBounds();
    }
    private void GameUpdate()
    {
        UpdateLevelBounds();
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
        return null;
    }
    private Block CreateBlock(Vector3Int gridPos)
    {
        Block block = Instantiate(blockPrefab, grid.CellToWorld(gridPos), Quaternion.identity, transform).GetComponent<Block>();
        block.Randomize();
        return block;
    }
    private Block CloneBlock(Block block, Vector3Int gridPos)
    {
        Block clone = Instantiate(block, grid.CellToWorld(gridPos), Quaternion.identity, transform).GetComponent<Block>();
        clone.roomCode = block.roomCode;
        return clone;
    }
    private bool DestroyBlock(Vector3Int gridPos)
    {
        Block block = GetBlock(gridPos);
        if (block != null)
        {
            if (!block.fading)
            {
                Destroy(block.gameObject);
                return true;
            }
        }
        Debug.LogWarning("Attempted to destroy nonexistent block at " + gridPos, transform);
        return false;
    }
    private void ClearLevel()
    {
        foreach (Transform child in transform)
        {
            Block block = child.GetComponent<Block>();
            if (block != null)
            {
                Destroy(block.gameObject);
            }
        }
    }

    // Add or remove blocks so that the level has the proper bounds
    public void UpdateLevelBounds()
    {
        // update bounds variables
        topRow = grid.WorldToCell(Camera.main.ViewportToWorldPoint(Vector3.up)).y + 1;
        bottomRow = grid.WorldToCell(Camera.main.ViewportToWorldPoint(Vector3.zero)).y;
        if (GameManager.instance.currentScene == SceneID.title)
        {
            levelWidth = grid.WorldToCell(Camera.main.ViewportToWorldPoint(Vector3.right)).x + 1;
        }

        // Remove all blocks which are out of bounds
        foreach (Transform child in transform)
        {
            Block block = child.GetComponent<Block>();
            if (block != null)
            {
                if (!InBounds(block.gridPos) && !block.fading)
                {
;                   DestroyBlock(block.gridPos);
                }
            }
        }

        // Make sure bounds are filled with blocks
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = bottomRow; y < topRow; y++)
            {
                Vector3Int pos = new Vector3Int(x, y);
                if (GetBlock(pos) == null)
                {
                    CreateBlock(pos);
                }
            }
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

    // Shifts the nth column/row by shift, which should be either 1 or -1
    public bool ShiftColumn(int n, int shift)
    {
        List<Block> column = GetColumn(n);
        foreach (Block block in column)
        {
            if (block.shifting)
            {
                return false;
            }
        }
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
    public bool ShiftRow(int n, int shift)
    {
        List<Block> row = GetRow(n);
        foreach (Block block in row)
        {
            if (block.shifting)
            {
                return false;
            }
        }
        Block addedBlock = null;
        if (shift == 1)
        {
            addedBlock = CloneBlock(GetBlock(new Vector3Int(levelWidth - 1, n)), new Vector3Int(-1, n));
        }
        else if (shift == -1)
        {
            addedBlock = CloneBlock(GetBlock(new Vector3Int(0, n)), new Vector3Int(levelWidth, n));
        }
        else
        {
            Debug.LogWarning("ShiftRow() called with invalid shift value", transform);
        }
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

    public bool RandomShift()
    {
        Vector3Int[] compass = { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down };
        Vector3Int shift = compass[Random.Range(0, 4)];
        Vector3Int shiftPos = new Vector3Int(Random.Range(0, levelWidth), Random.Range(bottomRow, topRow));
        return ShiftFrom(shiftPos, shift);
    }

    // Whether or not a gridPos is inside the bounds of the current level
    public bool InBounds(Vector3Int gridPos)
    {
        return (gridPos.x >= 0) && (gridPos.y >= bottomRow) && (gridPos.x < levelWidth) && (gridPos.y < topRow);
    }

    // The center of the block at gridPos in world space
    public Vector3 CenterOfBlock(Vector3Int gridPos)
    {
        return grid.CellToWorld(gridPos) + grid.cellSize/2;
    }
}
