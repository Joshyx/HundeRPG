using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AudioClip clickSound;

    public void LoadProgressMode()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        MenuController.isInEndlessMode = false;
        SceneManager.LoadScene(1);
    }

    public void LoadEndlessModeOberstedten()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        MenuController.isInEndlessMode = true;
        SceneManager.LoadScene(1);
    }

    public void LoadEndlessModeNature()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        MenuController.isInEndlessMode = true;
        SceneManager.LoadScene(2);
    }

    public void LoadEndlessModeIsland()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        MenuController.isInEndlessMode = true;
        SceneManager.LoadScene(3);
    }

    public void ExitGame()
    {
        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
        Application.Quit();
    }
}
