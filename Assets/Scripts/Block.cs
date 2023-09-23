using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Block : MonoBehaviour
{
    public Tile wallTile;
    public GameObject manaPrefab;

    public Vector3Int gridPos { get; private set; }
    public bool fading { get; private set; } // Whether or not the block is in the process of being deleted

    private Grid grid;
    private Tilemap tilemap;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        tilemap = GetComponentInChildren<Tilemap>();

        fading = false;

        gridPos = LevelController.instance.grid.WorldToCell(transform.position);
        Generate();
    }

    // Randomly fill in the 4 side walls and create mana pickup
    public void Generate()
    {
        Vector3Int[] compass = { Vector3Int.right, Vector3Int.down, Vector3Int.up, Vector3Int.left };
        foreach (Vector3Int dir in compass)
        {
            if (Random.Range(0f, 1f) < LevelController.instance.density)
            {
                tilemap.SetTile(new Vector3Int(1, 1) + dir, wallTile);
            }
        }
        if (Random.Range(0f, 1f) <= LevelController.instance.manaChance)
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
        while (Time.time < startTime + LevelController.instance.shiftTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, (Time.time - startTime) / LevelController.instance.shiftTime);
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
        yield return new WaitForSeconds(LevelController.instance.shiftTime);
        Destroy(gameObject);
    }

    // Returns true if there is not wall on the side if this block in direction dir
    public bool IsOpen(Vector3Int dir)
    {
        return !tilemap.HasTile(new Vector3Int(1, 1) + dir);
    }
}