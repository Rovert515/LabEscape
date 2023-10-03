using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneID
{
    title,
    game
}

public enum PlayState
{
    playing,
    paused,
    loading
}

public enum Difficulty
{
    Dynamic,
    Easy,
    Medium,
    Hard,
    Puzzle,
    Blitz
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public delegate void GameEvent();
    public event GameEvent initializeLevel;
    public event GameEvent initializeOthers;
    public event GameEvent gameUpdate;

    public SettingsPreset settingsPreset;
    public SceneID currentScene;

    private PlayState playState;

    public GameSettings settings { get; private set; }
    public float gameTime { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        settings = GameSettings.presets[settingsPreset];
        InitializeScene(currentScene);
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Dynamic:
                settingsPreset = SettingsPreset.dynamic;
                break;
            case Difficulty.Easy:
                settingsPreset = SettingsPreset.easy;
                break;
            case Difficulty.Medium:
                settingsPreset = SettingsPreset.medium;
                break;
            case Difficulty.Hard:
                settingsPreset = SettingsPreset.hard;
                break;
            case Difficulty.Puzzle:
                settingsPreset = SettingsPreset.puzzle;
                break;
            case Difficulty.Blitz:
                settingsPreset = SettingsPreset.blitz;
                break;
        }
    }
    public void LoadScene(SceneID scene)
    {
        AsyncOperation asyncLoad;
        playState = PlayState.loading;
        asyncLoad = SceneManager.LoadSceneAsync((int) scene);
        // Loading bar here???
        asyncLoad.completed += operation =>
        {
            InitializeScene(scene);
        };
    }
    private void InitializeScene(SceneID scene)
    {
        currentScene = scene;
        switch (scene)
        {
            case SceneID.title:
                gameTime = 0;
                LevelController.instance.levelWidth = 10;
                LevelController.instance.transform.position = Camera.main.ScreenToWorldPoint(Vector3.zero) + Vector3.forward * 10;
                if (initializeLevel != null)
                {
                    initializeLevel();
                }
                break;
            case SceneID.game:
                gameTime = 0;
                playState = PlayState.playing;
                settings = GameSettings.presets[settingsPreset];
                LevelController.instance.levelWidth = settings.levelWidth;
                if (initializeLevel != null)
                {
                    initializeLevel();
                }
                if (initializeOthers != null)
                {
                    initializeOthers();
                }
                break;
        }
    }

    private void Update()
    {
        switch (currentScene)
        {
            case SceneID.title:
                gameTime += Time.deltaTime;
                if (gameUpdate != null)
                {
                    gameUpdate();
                }
                LevelController.instance.transform.position = Camera.main.ScreenToWorldPoint(Vector3.zero) + Vector3.forward * 10;
                if (Random.Range(0f, 1f) < 0.01)
                {
                    LevelController.instance.RandomShift();
                }
                break;
            case SceneID.game:
                switch (playState)
                {
                    case PlayState.playing:
                        gameTime += Time.deltaTime;
                        if (gameUpdate != null)
                        {
                            gameUpdate();
                        }
                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            playState = PlayState.paused;
                        }
                        break;
                    case PlayState.paused:
                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            playState = PlayState.playing;
                        }
                        if (Input.GetKeyDown(KeyCode.M))
                        {
                            LoadScene(SceneID.title);
                        }
                        break;
                }
                break;
        }
    }
}