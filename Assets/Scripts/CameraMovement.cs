using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float followSpeed;

    private Vector3 desiredPos;
    private Camera cam;
    float bottomOfLevel;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void Start()
    {
        bottomOfLevel = LevelController.instance.grid.CellToWorld(new Vector3Int(0, LevelController.instance.bottomRow)).y;
        desiredPos = transform.position;
        desiredPos.y = bottomOfLevel + cam.orthographicSize;
        transform.position = desiredPos;
    }
    private void Update()
    {
        bottomOfLevel = LevelController.instance.grid.CellToWorld(new Vector3Int(0, LevelController.instance.bottomRow)).y;
        desiredPos.y = PlayerMovement.instance.transform.position.y;
        if (desiredPos.y < bottomOfLevel + cam.orthographicSize)
        {
            desiredPos.y = bottomOfLevel + cam.orthographicSize;
        }

        transform.position += (desiredPos - transform.position) * followSpeed * Time.deltaTime;


    }
}
