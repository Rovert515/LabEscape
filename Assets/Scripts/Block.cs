using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Block : MonoBehaviour
{
    public GameObject manaPrefab;

    public Vector3Int gridPos { get; private set; }
    public bool fading { get; private set; } // Whether or not the block is in the process of being deleted

    private Grid grid;
    private string roomCode;

    private void Awake()
    {
        grid = GetComponent<Grid>();

        fading = false;

        gridPos = LevelController.instance.grid.WorldToCell(transform.position);
        Generate();
    }

    // Randomly fill in the 4 side walls and create mana pickup
    public void Generate()
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
        GameObject tilemapPrefab = Resources.Load<GameObject>("Block Themes/Lab/room_" + roomCode);
        if (tilemapPrefab == null) {
            Debug.LogWarning("Failed to find tilemap prefab at Block Themes/Lab/room_" + roomCode, transform);
        }
        else
        {
            Instantiate(tilemapPrefab, transform);
        }
        if (Random.Range(0f, 1f) <= GameManager.instance.settings.manaChance.GetValue())
        {
            Instantiate(manaPrefab, LevelController.instance.CenterOfBlock(gridPos), Quaternion.identity, transform);
        }
    }

    // Slide the block at a constant speed to gridPos + shift
    public void Shift(Vector3Int shift)
    {
        gridPos += shift;
        StartCoroutine(ShiftRoutine());
    }
    IEnumerator ShiftRoutine()
    {
        float startTime = Time.time;
        Vector3 targetPos = LevelController.instance.grid.CellToWorld(gridPos);
        Vector3 startPos = transform.position;
        while (Time.time < startTime + GameManager.instance.settings.shiftTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, (Time.time - startTime) / GameManager.instance.settings.shiftTime);
            yield return null;
        }
        transform.position = targetPos;
    }

    // Destroy the block once it finishes shifting
    public void Fade()
    {
        fading = true;
        StartCoroutine(FadeRoutine());
    }
    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(GameManager.instance.settings.shiftTime);
        Destroy(gameObject);
    }

    // Returns true if there is a wall on the side of this block in direction dir
    public bool GetWall(Vector3Int dir)
    {
        Dictionary<Vector3Int, int> dirToIndex = new Dictionary<Vector3Int, int>() { { Vector3Int.right, 0 }, { Vector3Int.up, 1 }, { Vector3Int.left, 2 }, { Vector3Int.down, 3 } };
        return roomCode[dirToIndex[dir]] == '1';
    }
}