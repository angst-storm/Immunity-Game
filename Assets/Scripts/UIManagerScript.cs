using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour
{
    public static bool learn;
    private static bool pauseGame;

    public static bool PauseGame
    {
        get => pauseGame;
        set
        {
            Time.timeScale = value ? 0 : 1;
            pauseGame = value;
        }
    }

    public void ChangeScene(int sceneIndex)
    {
        PauseGame = false;
        SceneManager.LoadScene(sceneIndex);
    }

    public void AddScene(int sceneIndex)
    {
        if (!PauseGame) Time.timeScale = 0;
        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    }

    public void CloseAddedScene(int sceneIndex)
    {
        SceneManager.UnloadSceneAsync(sceneIndex);
        if (!PauseGame) Time.timeScale = 1;
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