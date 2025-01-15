using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadProgressMode()
    {
        MenuController.isInEndlessMode = false;
        SceneManager.LoadScene(1);
    }

    public void LoadEndlessModeOberstedten()
    {
        MenuController.isInEndlessMode = true;
        SceneManager.LoadScene(1);
    }

    public void LoadEndlessModeNature()
    {
        MenuController.isInEndlessMode = true;
        SceneManager.LoadScene(2);
    }

    public void LoadEndlessModeIsland()
    {
        MenuController.isInEndlessMode = true;
        SceneManager.LoadScene(3);
    }
}
