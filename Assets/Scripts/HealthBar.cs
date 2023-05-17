using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private static Slider slider;
    private static Health health;
    public GameObject player;
    public void Start()
    {
        health = player.GetComponent<Health>();
        slider = GetComponent<Slider>();
        slider.maxValue = health.GetAmount();
    }

    public void Update()
    {
        slider.value = health.GetAmount();
    }
}
