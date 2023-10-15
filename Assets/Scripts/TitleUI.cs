using UnityEngine;

public class TitleUI : MonoBehaviour
{
    // Functions to be called by the buttons
    public void PlayGame()
    {
        GameManager.instance.LoadScene(SceneID.game);
    }
    public void DiffDynamic()
    {
        GameManager.instance.SetSettings(SettingsPreset.dynamic);
    }
    public void DiffEasy()
    {
        GameManager.instance.SetSettings(SettingsPreset.easy);
    }
    public void DiffMedium()
    {
        GameManager.instance.SetSettings(SettingsPreset.medium);
    }
    public void DiffHard()
    {
        GameManager.instance.SetSettings(SettingsPreset.hard);
    }
    public void DiffPuzzle()
    {
        GameManager.instance.SetSettings(SettingsPreset.puzzle);
    }
    public void DiffBlitz()
    {
        GameManager.instance.SetSettings(SettingsPreset.blitz);
    }
    public void ButtonPress()
    {
        SoundManager.instance.ButtonPress();
    }
}
