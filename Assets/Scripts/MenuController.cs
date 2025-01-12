using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject pauseScreen;
    public GameObject levelUpScreen;
    public GameObject hud;
    
    public TextMeshProUGUI levelUpText;
    public Button templateUpgradeButton;
    public TextMeshProUGUI templateUpgradeButtonTitle;
    public TextMeshProUGUI templateUpgradeButtonDescription;
    public Image templateUpgradeButtonImage;
    public GameObject upgradeButtonHorizontalView;
    
    private static bool paused = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused() && pauseScreen.activeSelf)
            {
                ContinuePausedGame();
            }
            else if(!IsGamePaused() && !pauseScreen.activeSelf)
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
        hud.SetActive(false);
    }

    public void ContinuePausedGame()
    {
        paused = false;
        pauseScreen.SetActive(false);
        levelUpScreen.SetActive(false);
        hud.SetActive(true);
    }

    public void ShowLevelUpScreen(int newLevel, List<Upgrade> upgrades, Action<Upgrade> onLevelUp)
    {
        paused = true;
        levelUpScreen.SetActive(true);
        levelUpText.text = "Level Up!: Level " + newLevel;

        if (upgrades.Count <= 0)
        {
            templateUpgradeButtonTitle.text = "Continue Game";
            templateUpgradeButtonImage.enabled = false;
            templateUpgradeButtonDescription.text = "You played for so long that no further upgrades are available!";
            var obj = Instantiate(templateUpgradeButton, upgradeButtonHorizontalView.transform, false);
            obj.gameObject.SetActive(true);
            obj.onClick.AddListener(() =>
            {
                ContinuePausedGame();
                for (int i = 0; i < upgradeButtonHorizontalView.transform.childCount; i++)
                {
                    Destroy(upgradeButtonHorizontalView.transform.GetChild(i).gameObject);
                }
            });
            return;
        }
        foreach (var upgrade in upgrades)
        {
            templateUpgradeButtonTitle.text = upgrade.name;
            templateUpgradeButtonDescription.text = upgrade.description;
            templateUpgradeButtonImage.sprite = upgrade.icon;
            var obj = Instantiate(templateUpgradeButton, upgradeButtonHorizontalView.transform, false);
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

    public GameObject templateLeaderboardEntry;
    public TextMeshProUGUI templateLeaderboardEntryNumber;
    public TextMeshProUGUI templateLeaderboardEntryName;
    public TextMeshProUGUI templateLeaderboardEntryLevel;
    public TextMeshProUGUI templateLeaderboardEntryXP;
    public GameObject leaderBoardLayoutGroup;
    
    private bool isGameOver = false;
    public void GameOver()
    {
        paused = true;
        deathScreen.SetActive(true);
        isGameOver = true;
    }
    private async void FixedUpdate()
    {
        if(!isGameOver) return;
        isGameOver = false;
        
        await Leaderboard.AddScore();
        
        var scores = await Leaderboard.GetScores();
        scores.ForEach(score =>
        {
            templateLeaderboardEntryNumber.text = score.rank.ToString();
            templateLeaderboardEntryName.text = score.playerName.Split("#")[0];
            templateLeaderboardEntryLevel.text = score.level.ToString();
            templateLeaderboardEntryXP.text = score.xp.ToString();
            var obj = Instantiate(templateLeaderboardEntry, leaderBoardLayoutGroup.transform, false);
            obj.SetActive(true);
        });
    }

    public GameObject startGameScreen;

    public void StartGame()
    {
        var playerName = startGameScreen.GetComponentInChildren<TMP_InputField>().text;
        if (playerName.Length < 3) return;
        Leaderboard.Login(playerName);
        paused = false;
        startGameScreen.SetActive(false);
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
