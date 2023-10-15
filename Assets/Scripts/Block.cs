using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Block : MonoBehaviour
{
    public GameObject keycardPrefab;
    public string roomCode; // A string of 0s and 1s which keeps track of which sides of the room have walls

    public Vector3Int gridPos { get; private set; }
    public bool fading { get; private set; } // Whether or not the block is in the process of being deleted
    public bool shifting { get; private set; } // Whether or not the block is currently shifting

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
            // Check if the block is ready to stop shifting, otherwise keep moving
            if (GameManager.instance.gameTime >= shiftStartTime + GameManager.instance.settings.shiftTime)
            {
                shifting = false;
                transform.position = LevelController.instance.grid.CellToWorld(gridPos);

                // If this block is being ridden by the player, tell the player to stop riding the block
                if (PlayerMovement.instance != null)
                {
                    if (PlayerMovement.instance.gridPos == gridPos)
                    {
                        PlayerMovement.instance.StopRiding();
                    }
                }

                // If the block is fading, destroy it when it finishes shifting
                if (fading)
                {
                    // Make sure we don't destroy the player if it is our child
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
                // Move towards gridPos
                transform.position = Vector3.Lerp(shiftStartPos, LevelController.instance.grid.CellToWorld(gridPos), (GameManager.instance.gameTime - shiftStartTime) / GameManager.instance.settings.shiftTime);
            }
        }
    }

    // Randomly generate a room code, fill in the 4 side walls, and create keycard pickup
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
        if (Random.Range(0f, 1f) <= GameManager.instance.settings.keycardChance.GetValue())
        {
            Keycard keycard = Instantiate(keycardPrefab, LevelController.instance.CenterOfBlock(gridPos), Quaternion.identity, transform).GetComponent<Keycard>();
            keycard.Randomize();
        }
    }

    // Use roomCode to find the correct tilemap prefab, then instantiate it and make it our child
    public void MakeTilemap()
    {
        GameObject tilemapPrefab = Resources.Load<GameObject>("Block Tilemap Prefabs/room_" + roomCode);
        if (tilemapPrefab == null)
        {
            Debug.LogWarning("Failed to find tilemap prefab at Block Tilemap Prefabs/room_" + roomCode, transform);
        }
        else
        {
            Tilemap tilemap = Instantiate(tilemapPrefab, transform).GetComponent<Tilemap>();
            tilemap.color = (Color) GameManager.instance.settings.blockColor;
        }
    }

    // Slide the block at a constant speed to gridPos + shift
    public bool Shift(Vector3Int shift)
    {
        if (!shifting)
        {
            shifting = true; 
            shiftStartTime = GameManager.instance.gameTime;
            shiftStartPos = transform.position;

            // If the player is on this block, tell them to start riding
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