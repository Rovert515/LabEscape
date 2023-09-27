using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public SettingsPreset settingsPreset;

    private Lava lava;
    private CameraMovement cam;
    private Border border;

    public GameSettings settings { get; private set; }

    private void Awake()
    {
        instance = this;

        settings = GameSettings.presets[settingsPreset];
        lava = GetComponentInChildren<Lava>();
        cam = GetComponentInChildren<CameraMovement>();
        border = GetComponentInChildren<Border>();
    }
    private void Start()
    {
        LevelController.instance.Initialize();
        PlayerMovement.instance.Initialize();
        PlayerController.instance.Initialize();
        cam.Initialize();
        lava.Initialize();
        border.Initialize();
        UIManager.instance.UpdateUI();
        
    }
    private void Update()
    {
        LevelController.instance.UpdateRows();
    }
}
