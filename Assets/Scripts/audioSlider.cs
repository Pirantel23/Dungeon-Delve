using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audioSlider : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("volume");
    }
}
