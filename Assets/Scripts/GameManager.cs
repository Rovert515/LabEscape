using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneID // These should be in the same order as the scenes are in the build manager
{
    title,
    game
}

public enum PlayState
{
    playing,
    paused,
    gameOver,
    loading
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public delegate void GameEvent();
    public event GameEvent initializeLevel;
    public event GameEvent initializeOthers;
    public event GameEvent gameUpdate;

    public SceneID currentScene;
    public SettingsPreset settingsPreset;
    public PlayState playState;

    public GameSettings settings { get; private set; }
    public float gameTime { get; private set; }

    private GameUI gameUI;

    private void Awake()
    {
        // Set up singleton, or self destruct if there is another "Managers" object coming in from another scene
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
        SetSettings(settingsPreset);
        InitializeScene(currentScene);
    }

    // Change the difficulty and update some things in response
    public void SetSettings(SettingsPreset preset)
    {
        settings = GameSettings.presets[preset];

        // When choosing the difficulty on the title screen, we want to reload the level so that the color changes in the background
        if (settingsPreset != preset)
        {
            if (currentScene == SceneID.title)
            {
                initializeLevel();
            }
        }

        // Change music accordingly
        switch (preset)
        {
            case SettingsPreset.easy:
                SoundManager.instance.SwitchMusic(0);
                break;
            case SettingsPreset.medium:
                SoundManager.instance.SwitchMusic(1);
                break;
            case SettingsPreset.hard:
                SoundManager.instance.SwitchMusic(2);
                break;
            default:
                SoundManager.instance.SwitchMusic(1);
                break;
        }

        settingsPreset = preset;
    }

    // Load a new scene and initialize it once it has been loaded
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

    // Initialize a scene; calls all of the initiazation functions of various scripts
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
                gameUI = GameObject.Find("UI").GetComponent<GameUI>();
                if (gameUI == null)
                {
                    Debug.LogError("Failed to find the GameUI script", transform);
                }
                ResumeGame();
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
                // Keep time moving
                gameTime += Time.deltaTime;
                if (gameUpdate != null)
                {
                    gameUpdate();
                }

                // Keep the level anchored in the lower left corner of the screen
                LevelController.instance.transform.position = Camera.main.ScreenToWorldPoint(Vector3.zero) + Vector3.forward * 10;

                // Occasionally shift the background
                if (Random.Range(0f, 1f) < 0.01)
                {
                    LevelController.instance.RandomShift();
                }
                break;
            case SceneID.game:
                switch (playState)
                {
                    case PlayState.playing:
                        // Keep time moving
                        gameTime += Time.deltaTime;
                        if (gameUpdate != null)
                        {
                            gameUpdate();
                        }

                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            PauseGame();
                        }
                        break;
                    case PlayState.paused:
                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            ResumeGame();
                        }
                        break;
                    case PlayState.gameOver:
                        if (Input.GetKeyDown(KeyCode.R))
                        {
                            LoadScene(SceneID.game);
                        }
                        break;
                }
                break;
        }
    }

    public void GameOver()
    {
        if (currentScene == SceneID.game)
        {
            gameUI.GameOverScreen();
            playState = PlayState.gameOver;
            SoundManager.instance.GameOver();
        }
    }

    public void PauseGame()
    {
        if (currentScene == SceneID.game)
        {
            gameUI.PausedScreen();
            playState = PlayState.paused;
        }
    }

    public void ResumeGame()
    {
        if (currentScene == SceneID.game)
        {
            gameUI.PlayingScreen();
            playState = PlayState.playing;
        }
    }
}

