
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Slider slider;

    public float globalValue = 10;

    public Sound[] sounds;

    private float oldValue;

    void Awake()
    {
        globalValue = PlayerPrefs.GetFloat("volume");
        oldValue = globalValue;
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * globalValue / 10;
            s.source.loop = s.loop;
        }
    }

    public void Play(SoundType sound)
    {
        var s = Array.Find(sounds, item => item.type == sound);
        s.source.Play();
    }

    public void Stop(SoundType sound)
    {
        var s = Array.Find(sounds, item => item.type == sound);
        s.source.Stop();
    }

    public void setValue()
    {
        globalValue = slider.value;
        if (oldValue == globalValue) return;
        PlayerPrefs.SetFloat("volume", oldValue);
        oldValue = globalValue;
        foreach (var s in sounds)
        {
            s.source.volume = s.volume * globalValue / 10;
        }
    }
}