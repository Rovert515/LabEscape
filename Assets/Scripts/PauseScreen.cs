using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public SpriteRenderer popUpSpriteRenderer; // Reference to the SpriteRenderer component
    public Sprite newSprite; // This is the sprite you want to assign.

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = newSprite; // Assign the new sprite.
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        popUpSpriteRenderer.enabled = true; // Show the sprite when pausing the game
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        popUpSpriteRenderer.enabled = false; // Hide the sprite when unpausing the game
    }

}
