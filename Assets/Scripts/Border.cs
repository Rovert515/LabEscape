using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Border : MonoBehaviour
{

    public Tile borderTile;

    private Tilemap tilemap;
    private void OnEnable()
    {
        GameManager.instance.initializeOthers += Initialize;
        //GameManager.instance.gameUpdate += GameUpdate;
    }
    private void OnDisable()
    {
        GameManager.instance.initializeOthers -= Initialize;
        //GameManager.instance.gameUpdate -= GameUpdate;
    }
    public void Initialize()
    {
        tilemap = GetComponent<Tilemap>();
        tilemap.ClearAllTiles();

        int levelHeight = LevelController.instance.topRow - LevelController.instance.bottomRow;
        int levelWidth = LevelController.instance.levelWidth;
        
        // fill in the left and right sides of the tilemap with border tiles
        for (int x = -levelWidth; x < 0; x++)
        {
            for (int y = -levelHeight; y < 2*levelHeight; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y), borderTile);
            }
        }
        for (int x = levelWidth; x < 2*levelWidth; x++)
        {
            for (int y = -levelHeight; y < 2*levelHeight; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y), borderTile);
            }
        }

        // make the border a child of the camera so that it stays on screen
        transform.parent = Camera.main.transform;
    }
}
