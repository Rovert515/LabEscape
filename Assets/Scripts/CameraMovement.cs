using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float followSpeed; // How tightly the camara follows the y pos of the player

    private Vector3 desiredPos;
    private float cameraHeight;

    private void Start()
    {
        cameraHeight = Camera.main.orthographicSize;
        desiredPos = transform.position;
        UpdateDesiredPos();
        transform.position = desiredPos;
    }
    private void Update()
    {
        UpdateDesiredPos();
        transform.position += (desiredPos - transform.position) * followSpeed * Time.deltaTime;
    }

    // Set the y value of desiredPos to the y value of the player, but don't show past the bottom of the level
    private void UpdateDesiredPos()
    {
        float bottomOfLevel = LevelController.instance.grid.CellToWorld(new Vector3Int(0, LevelController.instance.bottomRow)).y;
        desiredPos.y = PlayerMovement.instance.transform.position.y;
        if (desiredPos.y < bottomOfLevel + cameraHeight)
        {
            desiredPos.y = bottomOfLevel + cameraHeight;
        }
    }
}
