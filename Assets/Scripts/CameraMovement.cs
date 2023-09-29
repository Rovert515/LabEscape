using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{   
    public float followSpeed; // How tightly the camara follows the y pos of the player

    private Vector3 desiredPos;
    private float cameraHeight;
    private float bottomOfLevel;
    public float verticalOffset;

    private void OnEnable()
    {
        GameManager.instance.initializeOthers += Initialize;
        GameManager.instance.gameUpdate += GameUpdate;
    }
    private void OnDisable()
    {
        GameManager.instance.initializeOthers -= Initialize;
        GameManager.instance.gameUpdate -= GameUpdate;
    }
    public void Initialize()
    {
        cameraHeight = LevelController.instance.width*0.5f;
        Camera.main.orthographicSize = cameraHeight;
        bottomOfLevel = LevelController.instance.grid.CellToWorld(new Vector3Int(0, LevelController.instance.bottomRow)).y;
        transform.localPosition = new Vector3(LevelController.instance.width/2, LevelController.instance.width / 2 + verticalOffset, -10);
        desiredPos = transform.position;
    }
    private void GameUpdate()
    {
        UpdateDesiredPos();
        transform.position += (desiredPos - transform.position) * followSpeed * Time.deltaTime;
    }

    // Set the y value of desiredPos to the y value of the player, but don't show past the bottom of the level
    private void UpdateDesiredPos()
    {
        bottomOfLevel = LevelController.instance.grid.CellToWorld(new Vector3Int(0, LevelController.instance.bottomRow)).y;
        desiredPos.y = PlayerMovement.instance.transform.position.y + verticalOffset;
        desiredPos.y = Mathf.Clamp(desiredPos.y, bottomOfLevel + cameraHeight, Mathf.Infinity);
    }
}
