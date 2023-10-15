using UnityEngine;
using UnityEngine.UI;

public class ShiftBar : MonoBehaviour
{
    public Sprite[] sprites; // All the bar sprites in order

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
