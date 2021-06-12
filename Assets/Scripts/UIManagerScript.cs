using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour
{
    public static bool learn;

    public void ChangeScene(int sceneIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneIndex);
    }

    public void AddScene(int sceneIndex)
    {
        Time.timeScale = 0;
        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    }

    public void CloseAddedScene(int sceneIndex)
    {
        SceneManager.UnloadSceneAsync(sceneIndex);
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetBool(bool parameter)
    {
        learn = parameter;
    }
}