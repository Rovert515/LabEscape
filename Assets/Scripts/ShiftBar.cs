using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiftBar : MonoBehaviour
{
    public Sprite[] sprites;

    private Image myImage;

    private void Awake()
    {
        myImage = GetComponent<Image>();
    }
    
    public void SetValue(int value)
    {
        myImage.sprite = sprites[value];
    }
}
