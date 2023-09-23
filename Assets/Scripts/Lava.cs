using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float speed;

    public float height { get; private set; }

    private void Awake()
    {
        height = -10;
        UpdatePos();
    }
    private void Update()
    {
        // Lave moves 3x faster if it is offscreen
        if (height > Camera.main.transform.position.y - Camera.main.orthographicSize)
        {
            height += speed * Time.deltaTime;
        }
        else
        {
            height += 3 * speed * Time.deltaTime;
        }
        UpdatePos();
    }

    // Set transform.position to reflect height
    private void UpdatePos()
    {
        Vector3 newPos = transform.position;
        newPos.y = height - transform.localScale.y / 2;
        transform.position = newPos;
    }
}
