using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;

	public Sound[] sounds;

	void Awake ()
	{
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
			s.source.volume = s.volume;
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

}
