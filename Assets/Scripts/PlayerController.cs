using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

// Handles the input and manages the player's resources
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }
    public Lava lava;

    public int manaCount = 5;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        // Determine directional input
        Vector3Int inputDir = Vector3Int.zero;
        if (Input.GetButtonDown("Right"))
        {
            inputDir = Vector3Int.right;
        }
        else if (Input.GetButtonDown("Left"))
        {
            inputDir = Vector3Int.left;
        }
        else if (Input.GetButtonDown("Up"))
        {
            inputDir = Vector3Int.up;
        }
        else if (Input.GetButtonDown("Down"))
        {
            inputDir = Vector3Int.down;
        }

        // Attempt to either move or shift based on input
        if (inputDir != Vector3Int.zero)
        {
            if (Input.GetButton("Shift"))
            {
                // Attempt to shift if player has enough mana
                if (manaCount > 0)
                {
                    // If successful, consume a mana
                    if (PlayerMovement.instance.Shift(inputDir))
                    {
                        manaCount--;
                        UIManager.instance.UpdateUI();
                    }
                }
            }
            else
            {
                // Attempt to move
                if (PlayerMovement.instance.Move(inputDir))
                {
                    // Attempt to collect mana
                    Mana mana = LevelController.instance.GetBlock(PlayerMovement.instance.gridPos).GetComponentInChildren<Mana>();
                    if (mana != null)
                    {
                        manaCount += mana.value;
                        UIManager.instance.UpdateUI();
                        mana.Fade();
                    }
                }
            }
        }

        // Restart scene if player goes below the height of the lava
        if (transform.position.y < lava.height)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
