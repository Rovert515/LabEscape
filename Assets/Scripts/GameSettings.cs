#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

public enum SettingsPreset {
    dynamic,
    easy,
    medium,
    hard,
    puzzle,
    blitz,
    testing
}

public class GameSettings
{
    public static Dictionary<SettingsPreset, GameSettings> presets = new Dictionary<SettingsPreset, GameSettings>();

    public HeightBasedFloat density;
    public HeightBasedFloat manaChance;
    public int manaValue;
    public int startingMana;
    public float moveTime;
    public float shiftTime;
    public int levelWidth;
    public HeightBasedFloat lavaSpeed;
    public float lavaSpeedMultiplier;

    public GameSettings
        (HeightBasedFloat density,
        HeightBasedFloat manaChance,
        HeightBasedFloat lavaSpeed,
        int manaValue = 1,
        int startingMana = 5,
        float moveTime = 0.2f,
        float shiftTime = 0.2f,
        int levelWidth = 6,
        float lavaSpeedMultiplier = 3f)
    {
        this.density = density;
        this.manaChance = manaChance;
        this.lavaSpeed = lavaSpeed;
        this.manaValue = manaValue;
        this.startingMana = startingMana;
        this.moveTime = moveTime;
        this.shiftTime = shiftTime;
        this.levelWidth = levelWidth;
        this.lavaSpeedMultiplier = lavaSpeedMultiplier;
    }

    static GameSettings()
    {
        presets[SettingsPreset.testing] = new GameSettings(
            density: new HeightBasedFloat(0.2f),
            manaChance: new HeightBasedFloat(0.1f, 0.5f, max: 0.5f),
            lavaSpeed: new HeightBasedFloat(0f, 5f)
            );
        presets[SettingsPreset.dynamic] = new GameSettings(
            density: new HeightBasedFloat(0.3f, 0.025f, max: 0.375f),
            manaChance: new HeightBasedFloat(0.15f, -0.025f, min: 0.075f),
            lavaSpeed: new HeightBasedFloat(1f, 0.5f, max: 2.5f)
            );
        presets[SettingsPreset.easy] = new GameSettings(
            density: new HeightBasedFloat(0.3f),
            manaChance: new HeightBasedFloat(0.15f),
            lavaSpeed: new HeightBasedFloat(1f)
            );
        presets[SettingsPreset.medium] = new GameSettings(
            density: new HeightBasedFloat(0.325f),
            manaChance: new HeightBasedFloat(0.125f),
            lavaSpeed: new HeightBasedFloat(1.5f)
            );
        presets[SettingsPreset.hard] = new GameSettings(
            density: new HeightBasedFloat(0.35f),
            manaChance: new HeightBasedFloat(0.1f),
            lavaSpeed: new HeightBasedFloat(2f)
            );
        presets[SettingsPreset.puzzle] = new GameSettings(
            density: new HeightBasedFloat(0.4f),
            manaChance: new HeightBasedFloat(0.1f),
            lavaSpeed: new HeightBasedFloat(0f)
            );
        presets[SettingsPreset.blitz] = new GameSettings(
            density: new HeightBasedFloat(0.3f),
            manaChance: new HeightBasedFloat(0.1f),
            lavaSpeed: new HeightBasedFloat(5f),
            moveTime: 0.15f,
            shiftTime: 0.15f
            );
    }
}

public class HeightBasedFloat
{
    public float initialValue;
    public float changePerHeight; // change in value for every 100 height gained
    public float min;
    public float max;
    public HeightBasedFloat(float initialValue, float changePerHeight=0, float min=Mathf.NegativeInfinity, float max=Mathf.Infinity)
    {
        this.initialValue = initialValue;
        this.changePerHeight = changePerHeight;
        this.min = min;
        this.max = max;
    }
    public float GetValue(int height)
    {
        return Mathf.Clamp(initialValue + changePerHeight * height / 100, min, max);
    }
    public float GetValue()
    {
        return Mathf.Clamp(initialValue + changePerHeight * LevelController.instance.bottomRow / 100, min, max);
    }
}
