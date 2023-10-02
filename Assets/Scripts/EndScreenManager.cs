using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    public Canvas endScreenCanvas;

    void Start()
    {
        // Disable the end screen canvas initially
        endScreenCanvas.enabled = false;
    }

    public void ShowEndScreen()
    {
        // Enable the end screen canvas when called
        endScreenCanvas.enabled = true;
    }
}
