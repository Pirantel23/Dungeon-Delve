using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public void Click()
    {
        AudioManager.instance.Play(SoundType.ButtonClick);
    }
}
