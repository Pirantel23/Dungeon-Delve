using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image loadingScreen;
    public void LoadScene(int sceneIndex)
    {
        Debug.Log($"Loading scene {sceneIndex}");
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
}