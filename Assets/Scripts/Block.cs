using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Block : MonoBehaviour
{
    public GameObject manaPrefab;
    public string roomCode;

    public Vector3Int gridPos { get; private set; }
    public bool fading { get; private set; } // Whether or not the block is in the process of being deleted
    public bool shifting { get; private set; }

    private Grid grid;
    
    private float shiftStartTime;
    private Vector3 shiftStartPos;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        fading = false;
        shifting = false;
        gridPos = LevelController.instance.grid.WorldToCell(transform.position);
    }

 

    private void OnEnable()
    {
        //GameManager.instance.initializeLevel += Initialize;
        GameManager.instance.gameUpdate += GameUpdate;
    }
    private void OnDisable()
    {
        //GameManager.instance.initializeLevel -= Initialize;
        GameManager.instance.gameUpdate -= GameUpdate;
    }

    private void GameUpdate()
    {
        if (shifting)
        {
            if (GameManager.instance.gameTime >= shiftStartTime + GameManager.instance.settings.shiftTime)
            {
                shifting = false;
                transform.position = LevelController.instance.grid.CellToWorld(gridPos);
                if (PlayerMovement.instance != null)
                {
                    if (PlayerMovement.instance.gridPos == gridPos)
                    {
                        PlayerMovement.instance.StopRiding();
                    }
                }
                if (fading)
                {
                    PlayerMovement player = GetComponentInChildren<PlayerMovement>();
                    if (player != null)
                    {
                        player.transform.parent = LevelController.instance.transform;
                    }
                    Destroy(gameObject);
                }
            }
            else
            {
                // move towards gridPos
                transform.position = Vector3.Lerp(shiftStartPos, LevelController.instance.grid.CellToWorld(gridPos), (GameManager.instance.gameTime - shiftStartTime) / GameManager.instance.settings.shiftTime);
            }
        }
    }

    // Randomly fill in the 4 side walls and create keycard pickup
    public void Randomize()
    {
        roomCode = "";
        for (int i = 0; i < 4; i++)
        {
            if (Random.Range(0f, 1f) < GameManager.instance.settings.density.GetValue())
            {
                roomCode += "1";
            }
            else
            {
                roomCode += "0";
            }
        }
        if (roomCode == "1111")
        {
            char[] codeArray = roomCode.ToCharArray();
            codeArray[Random.Range(0, 4)] = '0';
            roomCode = new string(codeArray);
        }
        MakeTilemap();
        if (Random.Range(0f, 1f) <= GameManager.instance.settings.manaChance.GetValue())
        {
            Instantiate(manaPrefab, LevelController.instance.CenterOfBlock(gridPos), Quaternion.identity, transform);
        }
    }


    public void MakeTilemap()
    {
        GameObject tilemapPrefab = Resources.Load<GameObject>("Block Themes/Lab/room_" + roomCode);
        if (tilemapPrefab == null)
        {
            Debug.LogWarning("Failed to find tilemap prefab at Block Themes/Lab/room_" + roomCode, transform);
        }
        else
        {
            Tilemap tilemap = Instantiate(tilemapPrefab, transform).GetComponent<Tilemap>();
            tilemap.color = (Color) GameManager.instance.settings.blockColor;
        }
    }

    // GameManager.instance.settings.(colorChange or something)

    // Slide the block at a constant speed to gridPos + shift
    public bool Shift(Vector3Int shift)
    {
        if (!shifting)
        {
            shifting = true; 
            shiftStartTime = GameManager.instance.gameTime;
            shiftStartPos = transform.position;
            if (PlayerMovement.instance != null)
            {
                if (PlayerMovement.instance.gridPos == gridPos)
                {
                    PlayerMovement.instance.StartRiding(shift);
                }
            }
            gridPos += shift;
            return true;
        }
        return false;
    }

    // Destroy the block once it finishes shifting
    public void Fade()
    {
        fading = true;
    }

    // Returns true if there is a wall on the side of this block in direction dir
    public bool GetWall(Vector3Int dir)
    {
        Dictionary<Vector3Int, int> dirToIndex = new Dictionary<Vector3Int, int>() { { Vector3Int.right, 0 }, { Vector3Int.up, 1 }, { Vector3Int.left, 2 }, { Vector3Int.down, 3 } };
        return roomCode[dirToIndex[dir]] == '1';
    }
}