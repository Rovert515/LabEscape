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
    public HeightBasedFloat acidSpeed;
    public float acidSpeedMultiplier;
    public Color? blockColor;
    public float goldenChance;

    public GameSettings
        (HeightBasedFloat density,
        HeightBasedFloat keycardChance,
        HeightBasedFloat acidSpeed,
        int keycardValue = 1,
        int startingMana = 6,
        float moveTime = 0.25f,
        float shiftTime = 0.25f,
        int levelWidth = 5,
        float acidSpeedMultiplier = 2.5f,
        Color? blockColor = null,
        float goldenChance = 0.1f)
    {
        this.density = density;
        this.manaChance = keycardChance;
        this.acidSpeed = acidSpeed;
        this.manaValue = keycardValue;
        this.startingMana = startingMana;
        this.moveTime = moveTime;
        this.shiftTime = shiftTime;
        this.levelWidth = levelWidth;
        this.acidSpeedMultiplier = acidSpeedMultiplier;
        this.blockColor = blockColor;
        this.goldenChance = goldenChance;
    }

    static GameSettings()
    {
        presets[SettingsPreset.testing] = new GameSettings(
            density: new HeightBasedFloat(0.2f),
            keycardChance: new HeightBasedFloat(0.1f, 0.5f, max: 0.5f),
            acidSpeed: new HeightBasedFloat(0f, 5f)
            );
        presets[SettingsPreset.dynamic] = new GameSettings(
            density: new HeightBasedFloat(0.3f, 0.025f, max: 0.375f),
            keycardChance: new HeightBasedFloat(0.15f, -0.025f, min: 0.075f),
            acidSpeed: new HeightBasedFloat(1f, 0.5f, max: 2.5f)
            );
        presets[SettingsPreset.easy] = new GameSettings(
            density: new HeightBasedFloat(0.3f),
            keycardChance: new HeightBasedFloat(0.15f),
            acidSpeed: new HeightBasedFloat(1f),
            blockColor: Color.Lerp(Color.white, Color.green, 0.3f)
            );
        presets[SettingsPreset.medium] = new GameSettings(
            density: new HeightBasedFloat(0.325f),
            keycardChance: new HeightBasedFloat(0.125f),
            acidSpeed: new HeightBasedFloat(1.5f),
            blockColor: Color.Lerp(Color.white, Color.yellow, 0.3f)
            );
        presets[SettingsPreset.hard] = new GameSettings(
            density: new HeightBasedFloat(0.35f),
            keycardChance: new HeightBasedFloat(0.1f),
            acidSpeed: new HeightBasedFloat(2f),
            blockColor: Color.Lerp(Color.white, Color.red, 0.3f)
            );
        presets[SettingsPreset.puzzle] = new GameSettings(
            density: new HeightBasedFloat(0.4f),
            keycardChance: new HeightBasedFloat(0.1f),
            acidSpeed: new HeightBasedFloat(0f)
            );
        presets[SettingsPreset.blitz] = new GameSettings(
            density: new HeightBasedFloat(0.3f),
            keycardChance: new HeightBasedFloat(0.1f),
            acidSpeed: new HeightBasedFloat(5f),
            moveTime: 0.15f,
            shiftTime: 0.15f
            );
        foreach (GameSettings preset in presets.Values)
        {
            if (preset.blockColor == null)
            {
                preset.blockColor = Color.white;
            }
        }
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
