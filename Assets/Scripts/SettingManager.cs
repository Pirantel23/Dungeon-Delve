using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    
    /// <summary>
    /// open setting's panel
    /// </summary>
    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ClosePanel();
    }
    /// <summary>
    /// close setting's panek
    /// </summary>
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}