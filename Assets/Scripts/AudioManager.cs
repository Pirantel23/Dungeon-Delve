
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public float globalValue = 10;

    public Sound[] sounds;
    

    void Awake()
    {
        globalValue = PlayerPrefs.GetFloat("volume");
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
    /// <summary>
    /// Starts the specified audio type
    /// </summary>
    public void Play(SoundType sound)
    {
        var s = Array.Find(sounds, item => item.type == sound);
        s.source.Play();
    }
    /// <summary>
    /// Stops the specified audio type
    /// </summary>
    public void Stop(SoundType sound)
    {
        var s = Array.Find(sounds, item => item.type == sound);
        s.source.Stop();
    }
}