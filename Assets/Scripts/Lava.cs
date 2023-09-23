using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float speed;

    public float height { get; private set; }

    private CameraMovement cam;

    private void Awake()
    {
        cam = Camera.main.GetComponent<CameraMovement>();
        
        height = -10;
        UpdatePos();
    }
    private void Update()
    {

        if (height > cam.bottomOfScreen)
        {
            height += speed * Time.deltaTime;
        }
        else
        {
            height += 3 * speed * Time.deltaTime;
        }
        
        UpdatePos();
    }
    private void UpdatePos()
    {
        Vector3 newPos = transform.position;
        newPos.y = height - transform.localScale.y / 2;
        transform.position = newPos;
    }
}
