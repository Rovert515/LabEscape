using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.instance.LoadScene(SceneID.game);
    }

    public void DiffDynamic()
    {
        GameManager.instance.SetDifficulty(SettingsPreset.dynamic);
    }

    public void DiffEasy()
    {
        GameManager.instance.SetDifficulty(SettingsPreset.easy);
    }

    public void DiffMedium()
    {
        GameManager.instance.SetDifficulty(SettingsPreset.medium);
    }

    public void DiffHard()
    {
        GameManager.instance.SetDifficulty(SettingsPreset.hard);
    }

    public void DiffPuzzle()
    {
        GameManager.instance.SetDifficulty(SettingsPreset.puzzle);
    }

    public void DiffBlitz()
    {
        GameManager.instance.SetDifficulty(SettingsPreset.blitz);
    }
}
