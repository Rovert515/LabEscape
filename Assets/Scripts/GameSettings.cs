using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

public enum SettingsPreset {
    easy,
    medium,
    hard,
    puzzle,
    blitz
}

public class GameSettings
{
    public static Dictionary<SettingsPreset, GameSettings> presets = new Dictionary<SettingsPreset, GameSettings>();

    public float density;
    public float manaChance;
    public int manaValue;
    public int startingMana;
    public float moveTime;
    public float shiftTime;
    public int levelWidth;
    public float lavaSpeed;
    public float lavaSpeedMultiplier;

    public GameSettings
        (float density = 0.3f,
        float manaChance = 0.15f,
        int manaValue = 1,
        int startingMana = 5,
        float moveTime = 0.2f,
        float shiftTime = 0.2f,
        int levelWidth = 7,
        float lavaSpeed = 1f,
        float lavaSpeedMultiplier = 3f)
    {
        this.density = density;
        this.manaChance = manaChance;
        this.manaValue = manaValue;
        this.startingMana = startingMana;
        this.moveTime = moveTime;
        this.shiftTime = shiftTime;
        this.levelWidth = levelWidth;
        this.lavaSpeed = lavaSpeed;
        this.lavaSpeedMultiplier = lavaSpeedMultiplier;
    }

    static GameSettings()
    {
        presets[SettingsPreset.easy] = new GameSettings(density: 0.25f, manaChance: 0.2f, startingMana: 10, manaValue: 1);
        presets[SettingsPreset.medium] = new GameSettings(density: 0.35f, manaChance: 0.15f, lavaSpeed: 1.25f);
        presets[SettingsPreset.hard] = new GameSettings(density: 0.35f, manaChance: 0.1f, lavaSpeed: 2f);
        presets[SettingsPreset.puzzle] = new GameSettings(density: 0.4f, manaChance: 0.07f, lavaSpeed: 0f, manaValue: 1);
        presets[SettingsPreset.blitz] = new GameSettings(density: 0.25f, manaChance: 0.1f, lavaSpeed: 5f, moveTime: 0.15f, shiftTime: 0.15f);
    }
}
