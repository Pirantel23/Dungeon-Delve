using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateVolume : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public void setValue()
    {
        var value = slider.value;
        AudioManager.instance.globalValue = value;
        PlayerPrefs.SetFloat("volume", value);
        foreach (var s in AudioManager.instance.sounds)
        {
            s.source.volume = s.volume * value / 10;
        }
    }
}
