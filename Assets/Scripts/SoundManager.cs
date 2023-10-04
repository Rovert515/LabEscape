using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioClip easyMusic;
    private AudioClip buttonPress;
    private AudioClip pickup;

    private AudioSource[] musicSources = new AudioSource[2];
    private AudioSource oneShotSource;
    private int flip = 0;

    private double nextMusicStart;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        oneShotSource = GetComponent<AudioSource>();
        easyMusic = Resources.Load<AudioClip>("Sounds/easy music");
        buttonPress = Resources.Load<AudioClip>("Sounds/button press");
        pickup = Resources.Load<AudioClip>("Sounds/pickup");
    }

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject child = new GameObject("Music Source");
            child.transform.parent = gameObject.transform;
            musicSources[i] = child.AddComponent<AudioSource>();
            musicSources[i].volume = 0.7f;
        }

        nextMusicStart = AudioSettings.dspTime + 1f;
    }

    private void Update()
    {
        if (AudioSettings.dspTime > nextMusicStart - 1)
        {
            musicSources[flip].clip = easyMusic;
            musicSources[flip].PlayScheduled(nextMusicStart);
            flip = 1 - flip;
            nextMusicStart += easyMusic.length - 0.05;
        }
    }

    public void ButtonPress()
    {
        oneShotSource.PlayOneShot(buttonPress, 0.5f);
    }

    public void Pickup()
    {
        oneShotSource.PlayOneShot(pickup, 0.5f);
    }
}
