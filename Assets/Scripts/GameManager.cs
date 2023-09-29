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

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public delegate void GameEvent();
    public event GameEvent initializeLevel;
    public event GameEvent initializeOthers;
    public event GameEvent gameUpdate;

    public SettingsPreset settingsPreset;

    private SceneID currentScene;
    private PlayState playState;

    public GameSettings settings { get; private set; }
    public float gameTime { get; private set; }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        currentScene = SceneID.title;
        playState = PlayState.playing;
    }
    public void SetScene(SceneID sceneID)
    {
        currentScene = sceneID;
        switch (sceneID)
        {
            case SceneID.title:
                SceneManager.LoadScene(0);
                break;
            case SceneID.game:
                settings = GameSettings.presets[settingsPreset];
                gameTime = 0;
                playState = PlayState.loading;
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
                // Loading bar here???
                asyncLoad.completed += operation =>
                {
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
    public void PlayGame()
    {
        SetScene(SceneID.game);
    }
}