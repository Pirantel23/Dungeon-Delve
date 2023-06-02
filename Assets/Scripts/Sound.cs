using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
	ForkAttack,
	BackGround,
	PlayerDamage,
	ShovelAttack,
	WormDamage,
	ButtonClick,
	RangeWeapon,
	Laser,
	GolemDamage,
	MeleeGolem
}
[System.Serializable]
public class Sound {

	public SoundType type;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = 1;

	public bool loop = false;

	[HideInInspector]
	public AudioSource source;

}
