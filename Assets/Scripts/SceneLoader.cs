using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image loadingScreen;
    public void LoadScene(int sceneIndex)
    {
        Debug.Log($"Loading scene {sceneIndex}");
        AudioManager.instance.Stop(SoundType.BackGround);
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        if (loadingScreen is null) yield break;
        var operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            loadingScreen.color = new Color(255, 255, 255, operation.progress);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            LoadScene(2);
        }
    }
}