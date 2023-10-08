using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioClip easyMusic;
    private AudioClip mediumMusic;
    private AudioClip hardMusic;
    
    private AudioClip buttonPress;
    private AudioClip pickup;
    private AudioClip goodPickup;
    private AudioClip shiftSound;
    private AudioClip gameOver;

    private AudioSource[][] musicSources = { new AudioSource[2], new AudioSource[2], new AudioSource[2] };
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
        mediumMusic = Resources.Load<AudioClip>("Sounds/medium music");
        hardMusic = Resources.Load<AudioClip>("Sounds/hard music");
        buttonPress = Resources.Load<AudioClip>("Sounds/button press");
        pickup = Resources.Load<AudioClip>("Sounds/pickup");
        goodPickup = Resources.Load<AudioClip>("Sounds/good pickup");
        shiftSound = Resources.Load<AudioClip>("Sounds/shift");
        gameOver = Resources.Load<AudioClip>("Sounds/game over");

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject child = new GameObject("Music Source");
                child.transform.parent = gameObject.transform;
                musicSources[i][j] = child.AddComponent<AudioSource>();
                musicSources[i][j].volume = 0.2f;
            }
        }
        musicSources[0][0].clip = easyMusic;
        musicSources[0][1].clip = easyMusic;
        musicSources[1][0].clip = mediumMusic;
        musicSources[1][1].clip = mediumMusic;
        musicSources[2][0].clip = hardMusic;
        musicSources[2][1].clip = hardMusic;

        nextMusicStart = AudioSettings.dspTime + 1f;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (AudioSettings.dspTime > nextMusicStart - 1)
        {
            for (int i = 0; i < 3; i++)
            {
                musicSources[i][flip].PlayScheduled(nextMusicStart);
            }
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

    public void GoodPickup()
    {
        oneShotSource.PlayOneShot(goodPickup, 0.5f);
    }
    public void Shift()
    {
        oneShotSource.PlayOneShot(shiftSound, 0.6f);
    }
    public void GameOver()
    {
        oneShotSource.PlayOneShot(gameOver, 0.4f);
    }

    public void SwitchMusic(int newSong)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (i == newSong)
                {
                    musicSources[i][j].mute = false;
                }
                else
                {
                    musicSources[i][j].mute = true;
                }
            }
        }
    }
}
