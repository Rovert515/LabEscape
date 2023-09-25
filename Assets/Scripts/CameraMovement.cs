using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{   
    public float followSpeed; // How tightly the camara follows the y pos of the player

    private Vector3 desiredPos;
    private float cameraHeight;
    private float bottomOfLevel;

    public void Initialize()
    {
        cameraHeight = LevelController.instance.width*0.7f;
        Camera.main.orthographicSize = cameraHeight;
        bottomOfLevel = LevelController.instance.grid.CellToWorld(new Vector3Int(0, LevelController.instance.bottomRow)).y;
        transform.localPosition = new Vector3(LevelController.instance.width/2, Mathf.Clamp(PlayerMovement.instance.transform.position.y, bottomOfLevel + cameraHeight, Mathf.Infinity), -10);
        desiredPos = transform.position;
    }
    private void Update()
    {
        UpdateDesiredPos();
        transform.position += (desiredPos - transform.position) * followSpeed * Time.deltaTime;
    }

    // Set the y value of desiredPos to the y value of the player, but don't show past the bottom of the level
    private void UpdateDesiredPos()
    {
        bottomOfLevel = LevelController.instance.grid.CellToWorld(new Vector3Int(0, LevelController.instance.bottomRow)).y;
        desiredPos.y = PlayerMovement.instance.transform.position.y;
        desiredPos.y = Mathf.Clamp(desiredPos.y, bottomOfLevel + cameraHeight, Mathf.Infinity);
    }
}
