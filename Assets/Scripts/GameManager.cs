using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public SettingsPreset settingsPreset;

    public GameSettings settings { get; private set; }

    private void Awake()
    {
        instance = this;

        settings = GameSettings.presets[settingsPreset];
    }
}
