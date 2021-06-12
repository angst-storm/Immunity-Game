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
                LearnLaunch.learn = false;
                break;
            case 2:
                LearnLaunch.learn = true;
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