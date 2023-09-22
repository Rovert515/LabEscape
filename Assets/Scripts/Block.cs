using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Block : MonoBehaviour
{

    private Tilemap tilemap;
    public Tile wallTile;
    public GameObject keyPrefab;
    private LevelController levelController;
    private Grid myGrid;
    private Grid levelGrid;
    public Vector3 desiredPos;
    public bool doomed;
    private void Awake()
    {
        doomed = false;
        tilemap = GetComponentInChildren<Tilemap>();
        myGrid = GetComponent<Grid>();
        levelGrid = transform.parent.GetComponent<Grid>();
        levelController = GetComponentInParent<LevelController>();
        desiredPos = transform.position;
    }
    private void Start()
    {
        Generate();
    }
    private void Update()
    {
        if (doomed && Vector3.Distance(transform.position, desiredPos) < 0.01)
        {
            Destroy(gameObject);
        }
        transform.position += (desiredPos - transform.position) * Time.deltaTime * 10;
    }
    public void Generate()
    {
        Vector3Int[] compass = { Vector3Int.right, Vector3Int.down, Vector3Int.up, Vector3Int.left };
        foreach (Vector3Int dir in compass)
        {
            if (Random.Range(0f, 1f) <= levelController.density)
            {
                tilemap.SetTile(new Vector3Int(1, 1) + dir, wallTile);
            }
        }
        if (Random.Range(0f, 1f) <= levelController.keyChance)
        {
            Instantiate(keyPrefab, myGrid.CellToWorld(new Vector3Int(1, 1)) + myGrid.cellSize/2, Quaternion.identity, transform);
        }
    }
    public void Shift(Vector3Int shift)
    {
        Vector3 cellShift = levelGrid.cellSize + levelGrid.cellGap;
        desiredPos.x += shift.x * cellShift.x;
        desiredPos.y += shift.y * cellShift.y;
    }
    public void SetPosition()
    {
        transform.position = desiredPos;
    }
    public void SetPosition(Vector3 pos)
    {
        desiredPos = pos;
        transform.position = desiredPos;
    }
    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }
    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
    public Vector3Int GetGridPos()
    {
        return levelGrid.WorldToCell(desiredPos);
    }
}
