using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public bool isPause;
    [SerializeField]
    private GameObject pauseMenu;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (isPause) Resume();
        else Pause();
    }
    
    /// <summary>
    ///continue scene 
    /// </summary>
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPause = false;
    }
    /// <summary>
    /// pause scene
    /// </summary>
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPause = true;
    }
    /// <summary>
    /// back to main menu
    /// </summary>
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        
    }
}
