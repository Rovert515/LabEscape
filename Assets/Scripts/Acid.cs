using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Acid : MonoBehaviour
{
    public float height { get; private set; }
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
        height = -10;
        transform.localScale = new Vector3(LevelController.instance.width, 100, 1);
        transform.position = LevelController.instance.transform.position + Vector3.right * LevelController.instance.width / 2;
        UpdatePos();
    }
    private void GameUpdate()
    {
        // Lave moves 3x faster if it is offscreen
        if (height > Camera.main.transform.position.y - Camera.main.orthographicSize - 6f)
        {
            height += GameManager.instance.settings.acidSpeed.GetValue() * Time.deltaTime;
        }
        else
        {
            height += GameManager.instance.settings.acidSpeedMultiplier * GameManager.instance.settings.acidSpeed.GetValue() * Time.deltaTime;
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
