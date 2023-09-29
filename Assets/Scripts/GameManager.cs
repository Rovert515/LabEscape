using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneID
{
    start,
    title,
    game
}

public enum PlayState
{
    playing,
    paused,
    loading
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
        instance = this;
        DontDestroyOnLoad(gameObject);

        currentScene = SceneID.start;
        playState = PlayState.playing;
        settings = GameSettings.presets[settingsPreset];
    }
    private void Start()
    {
        SetScene(SceneID.title);
    }
    public void SetScene(SceneID sceneID)
    {
        AsyncOperation asyncLoad;
        switch (sceneID)
        {
            case SceneID.title:
                gameTime = 0;
                asyncLoad = SceneManager.LoadSceneAsync(0);
                asyncLoad.completed += operation =>
                {
                    currentScene = sceneID;
                    LevelController.instance.levelWidth = 10;
                    LevelController.instance.transform.position = Camera.main.ScreenToWorldPoint(Vector3.zero) + Vector3.forward * 10;
                    if (initializeLevel != null)
                    {
                        initializeLevel();
                    }
                };
                break;
            case SceneID.game:
                gameTime = 0;
                playState = PlayState.loading;
                asyncLoad = SceneManager.LoadSceneAsync(1);
                // Loading bar here???
                asyncLoad.completed += operation =>
                {
                    currentScene = sceneID;
                    LevelController.instance.levelWidth = settings.levelWidth;
                    if (initializeLevel != null)
                    {
                        initializeLevel();
                    }
                    if (initializeOthers != null)
                    {
                        initializeOthers();
                    }
                    playState = PlayState.playing;
                };
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
                        break;
                }
                break;
        }
    }
}