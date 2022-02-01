using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class AsyncSceneLoaderButton : MonoBehaviour
{
    [SerializeField] private Text loadingProgress;
    [SerializeField] private Button loadButton;
    [SerializeField] private string nameOfScene;

    void Start()
    {
        loadButton.onClick.AddListener(LoadButton);
    }

    private void LoadButton()
    {
        StartCoroutine(LoadScene(nameOfScene));
    }

    //public void LoadSceneByName()
    //{

    //}

    IEnumerator LoadScene(string sceneName)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.allowSceneActivation = false;

        Debug.Log("Pro :" + asyncOperation.progress);

        while (!asyncOperation.isDone)
        {
            loadingProgress.text = /*"Loading progress: " +*/ ((int)(asyncOperation.progress) * 100) + "%";

            if (asyncOperation.progress >= 0.9f)
            {
                //loadingProgress.text = "Press the space bar to continue";
                loadingProgress.text = "Ready!!!";

                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
