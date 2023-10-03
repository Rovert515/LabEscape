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
        GameManager.instance.SetDifficulty(Difficulty.Dynamic);
    }

    public void DiffEasy()
    {
        GameManager.instance.SetDifficulty(Difficulty.Easy);
    }

    public void DiffMedium()
    {
        GameManager.instance.SetDifficulty(Difficulty.Medium);
    }

    public void DiffHard()
    {
        GameManager.instance.SetDifficulty(Difficulty.Hard);
    }

    public void DiffPuzzle()
    {
        GameManager.instance.SetDifficulty(Difficulty.Puzzle);
    }

    public void DiffBlitz()
    {
        GameManager.instance.SetDifficulty(Difficulty.Blitz);
    }
}
