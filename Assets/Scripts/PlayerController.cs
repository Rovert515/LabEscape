using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }

    public int manaCount = 5;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (!PlayerMovement.instance.moving && !LevelController.instance.shifting)
        {
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
            if (inputDir != Vector3Int.zero)
            {
                if (Input.GetButton("Shift"))
                {
                    if (manaCount > 0)
                    {
                        if (PlayerMovement.instance.Shift(inputDir))
                        {
                            manaCount--;
                            UIManager.instance.UpdateUI();
                        }
                    }
                }
                else
                {
                    PlayerMovement.instance.Move(inputDir);
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
    }
}
