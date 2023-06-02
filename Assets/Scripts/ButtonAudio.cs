using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    /// <summary>
    /// Reproduces the sound of button's click on click
    /// </summary>
    public void Click()
    {
        AudioManager.instance.Play(SoundType.ButtonClick);
    }
}
