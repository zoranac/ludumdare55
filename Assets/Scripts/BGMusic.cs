using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    public AudioClip BaseBGMusic;
    public AudioSource AudioSource;

    public static BGMusic Instance;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }

    public void PlayClip(AudioClip clip)
    {
        AudioSource.clip = clip;
        AudioSource.Play();
    }

    public void PlayBaseMusic()
    {
        AudioSource.clip = BaseBGMusic;
        AudioSource.Play();
    }
}
