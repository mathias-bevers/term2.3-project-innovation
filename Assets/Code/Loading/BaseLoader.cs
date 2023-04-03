using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseLoader : NetworkingBehaviour
{
    [SerializeField] protected string sceneToLoad;

    bool _hasLoaded = false;
    protected bool hasLoaded { get => _hasLoaded; }

    bool isLoading = false;

    AsyncOperation operation;

    protected void LoadLevel()
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine("AsyncLoadLevel");

    }

    IEnumerator AsyncLoadLevel()
    {
        operation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                _hasLoaded = true;
                break;
            }
            yield return null;
        }
    }

    protected void Release()
    {
        if (operation == null) return;
        _hasLoaded = false;
        StartCoroutine("SceneSwitch");
    }

    IEnumerator SceneSwitch()
    {
        operation.allowSceneActivation = true;
         while(!operation.isDone) {
            yield return null;
        }
        SceneManager.UnloadSceneAsync("LoadingScene");
    }
}
