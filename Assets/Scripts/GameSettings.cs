#nullable enable

using System.Collections.Generic;
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

    public HeightBasedFloat density; // Chance of a room having a wall
    public HeightBasedFloat keycardChance;
    public int keycardValue;
    public int startingKeycards;
    public float moveTime; // Seconds it takes player to move
    public float shiftTime; // Seconds it takes level to shift
    public int levelWidth; // How many rooms wide the level is
    public HeightBasedFloat acidSpeed;
    public float acidCatchUp; // How much extra speed to add per height that the lava is off the screen, scales with acidSpeed
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
        float acidCatchUp = 0.3f,
        Color? blockColor = null,
        float goldenChance = 0.1f)
    {
        this.density = density;
        this.keycardChance = keycardChance;
        this.acidSpeed = acidSpeed;
        this.keycardValue = keycardValue;
        this.startingKeycards = startingMana;
        this.moveTime = moveTime;
        this.shiftTime = shiftTime;
        this.levelWidth = levelWidth;
        this.acidCatchUp = acidCatchUp;
        this.blockColor = blockColor;
        this.goldenChance = goldenChance;
    }

    static GameSettings()
    {
        // Define the presets

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

        // Unused presets

        presets[SettingsPreset.testing] = new GameSettings(
            density: new HeightBasedFloat(0.2f),
            keycardChance: new HeightBasedFloat(0.1f, 0.5f, max: 0.5f),
            acidSpeed: new HeightBasedFloat(0f, 5f)
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

        // Make the default color white
        foreach (GameSettings preset in presets.Values)
        {
            if (preset.blockColor == null)
            {
                preset.blockColor = Color.white;
            }
        }
    }
}

// Class for a float value that will change as the player gains height
public class HeightBasedFloat
{
    public float initialValue;
    public float changePerHeight; // Change in value for every 100 height gained
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
