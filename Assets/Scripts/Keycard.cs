using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Keycard : MonoBehaviour
{
    public int value;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Determine if the card is golden
        if (Random.Range(0f, 1f) < GameManager.instance.settings.goldenChance)
        {
            value = 3;
            sr.color = Color.yellow;
        }
        else
        {
            value = 1;
        }
    }
}
