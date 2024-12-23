using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject pauseScreen;
    
    private static bool paused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused())
            {
                ContinuePausedGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public static bool IsGamePaused()
    {
        return paused;
    }

    private void PauseGame()
    {
        paused = true;
        pauseScreen.SetActive(true);
    }

    public void ContinuePausedGame()
    {
        paused = false;
        pauseScreen.SetActive(false);
    }

    public void GameOver()
    {
        paused = true;
        deathScreen.SetActive(true);
    }

    public void RestartGame()
    {
        paused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        paused = false;
        SceneManager.LoadScene("MainMenu");
    }
}
