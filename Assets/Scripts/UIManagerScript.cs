using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour
{
    public void ChangeScene(int sceneIndex)
    {
        Time.timeScale = 1;
        switch (sceneIndex)
        {
            case 1:
                LearnLaunch.Learn = false;
                break;
            case 2:
                LearnLaunch.Learn = true;
                sceneIndex--;
                break;
        }

        SceneManager.LoadScene(sceneIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }
}