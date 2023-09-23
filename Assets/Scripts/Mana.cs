using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Mana : MonoBehaviour
{
    public int value;

    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }
    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(PlayerMovement.instance.moveTime);
        Destroy(gameObject);
    }
}
