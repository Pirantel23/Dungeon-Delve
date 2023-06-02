using System;
using UnityEngine.UI;
using UnityEngine;

public class UpdateSlider : MonoBehaviour
{
    private Slider slider;
    public string name;
    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        slider.value = PlayerPrefs.GetInt($"{name}_T");
    }
}
