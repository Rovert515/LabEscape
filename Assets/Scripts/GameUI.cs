using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject playingUI;
    public GameObject pausedUI;
    public GameObject gameOverUI;
    public ShiftBar shiftBar;
    public TMP_Text scoreText;

    public static GameUI instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        GameManager.instance.initializeOthers += UpdateUI;
        GameManager.instance.gameUpdate += UpdateUI;
    }
    private void OnDisable()
    {
        GameManager.instance.initializeOthers -= UpdateUI;
        GameManager.instance.gameUpdate -= UpdateUI;
    }
    private void UpdateUI()
    {
        shiftBar.SetValue(PlayerController.instance.keycardCount);
        scoreText.text = LevelController.instance.bottomRow.ToString();
    }
    public void PlayingScreen()
    {
        playingUI.SetActive(true);
        pausedUI.SetActive(false);
        gameOverUI.SetActive(false);
    }
    public void PausedScreen()
    {
        playingUI.SetActive(false);
        pausedUI.SetActive(true);
        gameOverUI.SetActive(false);
    }
    public void GameOverScreen()
    {
        playingUI.SetActive(false);
        pausedUI.SetActive(false);
        gameOverUI.SetActive(true);
    }
    public void TryAgain()
    {
        GameManager.instance.LoadScene(SceneID.game);
    }
    public void Back()
    {
        GameManager.instance.LoadScene(SceneID.title);
    }
    public void ButtonPress()
    {
        SoundManager.instance.ButtonPress();
    }
    public void Pause()
    {
        GameManager.instance.PauseGame();
    }
    public void Resume()
    {
        GameManager.instance.ResumeGame();
    }
}
