using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject pauseScreen;
    public GameObject levelUpScreen;
    
    public TextMeshProUGUI levelUpText;
    public Button templateUpgradeButton;
    public GameObject upgradeButtonHorizontalView;
    
    private static bool paused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused() && pauseScreen.activeSelf)
            {
                ContinuePausedGame();
            }
            else if(!pauseScreen.activeSelf)
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
        levelUpScreen.SetActive(false);
    }

    public void ShowLevelUpScreen(int newLevel, List<Upgrade> upgrades, Action<Upgrade> onLevelUp)
    {
        if (upgrades.Count < 0) return;
        
        paused = true;
        levelUpScreen.SetActive(true);
        levelUpText.text = "Level Up!: Level " + newLevel;
        
        foreach (var upgrade in upgrades)
        {
            var obj = Instantiate(templateUpgradeButton, upgradeButtonHorizontalView.transform, false);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.name;
            obj.gameObject.SetActive(true);
            obj.onClick.AddListener(() =>
            {
                onLevelUp(upgrade);
                ContinuePausedGame();
                for (int i = 0; i < upgradeButtonHorizontalView.transform.childCount; i++)
                {
                    Destroy(upgradeButtonHorizontalView.transform.GetChild(i).gameObject);
                }
            });
        }
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
