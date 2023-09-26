using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float height { get; private set; }

    public void Initialize()
    {
        height = -10;
        transform.localScale = new Vector3(LevelController.instance.width, Camera.main.orthographicSize * 2, 1);
        transform.localPosition = new Vector3(LevelController.instance.width / 2, 0);
        UpdatePos();
    }
    private void Update()
    {
        // Lave moves 3x faster if it is offscreen
        if (height > Camera.main.transform.position.y - Camera.main.orthographicSize)
        {
            height += GameManager.instance.settings.lavaSpeed.GetValue() * Time.deltaTime;
        }
        else
        {
            height += GameManager.instance.settings.lavaSpeedMultiplier * GameManager.instance.settings.lavaSpeed.GetValue() * Time.deltaTime;
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
