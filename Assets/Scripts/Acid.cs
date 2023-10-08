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
        float speed;
        // Acid moves faster if it is offscreen
        if (height > Camera.main.transform.position.y - Camera.main.orthographicSize)
        {
            speed = GameManager.instance.settings.acidSpeed.GetValue();
        }
        else
        {
            float gridUnitsBelowScreen = (Camera.main.transform.position.y - Camera.main.orthographicSize - height) / LevelController.instance.cellShift.y;
            speed = GameManager.instance.settings.acidSpeed.GetValue() * (1 + GameManager.instance.settings.acidCatchUp * gridUnitsBelowScreen);
        }
        height += speed * Time.deltaTime;
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
