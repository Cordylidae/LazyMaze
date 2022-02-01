using Logic.Maze.MazeUtils;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AsyncSceneLoader : MonoBehaviour
{
    [SerializeField] private string nameOfScene;

    public void LoadAsync()
    {
        StartCoroutine(LoadScene(nameOfScene));
    }

    IEnumerator LoadScene(string sceneName)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        
        asyncOperation.allowSceneActivation = false;
        
        Debug.Log("Pro :" + asyncOperation.progress);
        
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
