using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
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

    public AudioSource menuMusic;
    public AudioSource gameMusic;
    
    private static bool paused;
    private bool offline = true;

    private void Start()
    {
        offline = Application.internetReachability == NetworkReachability.NotReachable;
        if (!AuthenticationService.Instance.IsSignedIn || offline)
        {
            menuMusic.Play();
            ShowStartScreen();
        }
        else
        {
            gameMusic.Play();
        }
    }

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

    private void SetIsPaused(bool newPaused)
    {
        paused = newPaused;
        Time.timeScale = newPaused ? 0f : 1f;
    }

    private void PauseGame()
    {
        SwitchToMenuMusic();
        SetIsPaused(true);
        pauseScreen.SetActive(true);
        hud.SetActive(false);
    }

    public void ContinuePausedGame()
    {
        SwitchToGameMusic();
        SetIsPaused(false);
        pauseScreen.SetActive(false);
        levelUpScreen.SetActive(false);
        hud.SetActive(true);
    }

    public void ShowLevelUpScreen(int newLevel, List<Upgrade> upgrades, Action<Upgrade> onLevelUp)
    {
        SwitchToMenuMusic();
        SetIsPaused(true);
        hud.SetActive(false);
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
        deathScreen.SetActive(true);
        isGameOver = true;
        SwitchToMenuMusic();
    }
    private async void FixedUpdate()
    {
        if(!isGameOver) return;
        isGameOver = false;
        
        hud.SetActive(false);
        SetIsPaused(true);
        if (offline) return;
        await Leaderboard.AddScore();
        
        var scores = await Leaderboard.GetScores();
        var playerScore = await Leaderboard.GetPlayerScore();
        var playerInTop10 = scores.Exists(score => score.playerId == playerScore.playerId);
        if (!playerInTop10)
        {
            scores[^1] = new LeaderboardScore(0, 0, 0, "---", "");
            scores[^2] = playerScore;
        }
        scores.ForEach(score =>
        {
            templateLeaderboardEntryName.fontStyle = score.playerId == playerScore.playerId ? FontStyles.Bold | FontStyles.Underline : FontStyles.Normal;
            templateLeaderboardEntryNumber.text = score.rank != 0 ? score.rank.ToString() : "";
            templateLeaderboardEntryName.text = score.playerName.Split("#")[0];
            templateLeaderboardEntryLevel.text = score.rank != 0 ? score.level.ToString() : "";
            templateLeaderboardEntryXP.text = score.rank != 0 ? score.xp.ToString() : "";
            var obj = Instantiate(templateLeaderboardEntry, leaderBoardLayoutGroup.transform, false);
            obj.SetActive(true);
        });
    }

    public GameObject startGameScreen;
    public TextMeshProUGUI startScreenOfflineNotificationText;

    private void ShowStartScreen()
    {
        SetIsPaused(true);
        hud.SetActive(false);
        startGameScreen.SetActive(true);
        startScreenOfflineNotificationText.gameObject.SetActive(offline);
    }
    public void StartGame()
    {
        if (!offline)
        {
            var playerName = startGameScreen.GetComponentInChildren<TMP_InputField>().text;
            if (playerName.Length is < 3 or > 20) return;
            Leaderboard.Login(playerName);
        }
        SetIsPaused(false);
        hud.SetActive(true);
        startGameScreen.SetActive(false);
        SwitchToGameMusic();
    }

    public void RestartGame()
    {
        SetIsPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SetIsPaused(false);
        SceneManager.LoadScene("MainMenu");
    }

    private async void SwitchToGameMusic()
    {
        gameMusic.Play();
        for (int i = 1; i <= 20; i++)
        {
            gameMusic.volume = i / 40f;
            menuMusic.volume = 1 - i / 20f;
            await Task.Delay(100);
        }
        menuMusic.Stop();
    }
    private async void SwitchToMenuMusic()
    {
        menuMusic.Play();
        for (int i = 1; i <= 20; i++)
        {
            menuMusic.volume = i / 20f;
            gameMusic.volume = 0.5f - i / 40f;
            await Task.Delay(100);
        }
        gameMusic.Stop();
    }
}
