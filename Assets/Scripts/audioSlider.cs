using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audioSlider : MonoBehaviour
{
    /// <summary>
    /// get value from PlayerPrefs
    /// </summary>
    private void OnEnable()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("volume");
    }
}
