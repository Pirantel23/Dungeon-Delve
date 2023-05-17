using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    private static TextMeshProUGUI text;
    private static int money;
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public static void ChangeValue(int value)
    {
        money += value;
        text.text = money.ToString();
    }
}
