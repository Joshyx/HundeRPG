using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static bool isInEndlessMode = false;
    public static float progressModeTimeSinceStart;
    
    public GameObject deathScreen;
    public GameObject pauseScreen;
    public GameObject levelUpScreen;

    public GameObject hud;
    public TextMeshProUGUI timerText;
    
    public TextMeshProUGUI levelUpText;
    public Button templateUpgradeButton;
    public TextMeshProUGUI templateUpgradeButtonTitle;
    public TextMeshProUGUI templateUpgradeButtonDescription;
    public Image templateUpgradeButtonImage;
    public GameObject upgradeButtonHorizontalView;

    public AudioSource menuMusic;
    public AudioSource gameMusic;
    public AudioClip clickSound;
    
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
        progressModeTimeSinceStart += Time.deltaTime * 1000;
        timerText.text = TimeSpan.FromMilliseconds(progressModeTimeSinceStart).ToString(@"mm\:ss\.fff");
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
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
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
        levelUpText.text = "Reached Level " + newLevel + "!";

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
    
    public GameObject templateSpeedrunLeaderboardEntryEndless;
    public TextMeshProUGUI templateSpeedrunLeaderboardEntryTimeEndless;
    public TextMeshProUGUI templateSpeedrunLeaderboardEntryNameEndless;
    public TextMeshProUGUI templateSpeedrunLeaderboardEntryRankEndless;
    public GameObject speedrunLeaderBoardLayoutGroupEndless;
    
    public GameObject progressModeDeathScreen;
    
    private bool isGameOver = false;
    private bool isGameFinished = false;
    public void GameOver()
    {
        if (!isInEndlessMode)
        {
            hud.SetActive(false);
            progressModeDeathScreen.SetActive(true);
            SwitchToMenuMusic();
            SetIsPaused(true);
            return;
        }
        deathScreen.SetActive(true);
        isGameOver = true;
        SwitchToMenuMusic();
    }
    private async void FixedUpdate()
    {
        if (isGameOver)
        {
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
                templateLeaderboardEntryName.fontStyle = score.playerId == playerScore.playerId
                    ? FontStyles.Bold | FontStyles.Underline
                    : FontStyles.Normal;
                templateLeaderboardEntryNumber.text = score.rank != 0 ? score.rank.ToString() : "";
                templateLeaderboardEntryName.text = score.playerName.Split("#")[0];
                templateLeaderboardEntryLevel.text = score.rank != 0 ? score.level.ToString() : "";
                templateLeaderboardEntryXP.text = score.rank != 0 ? score.xp.ToString() : "";
                var obj = Instantiate(templateLeaderboardEntry, leaderBoardLayoutGroup.transform, false);
                obj.SetActive(true);
            });
            
            await Leaderboard.AddTimeEndless();

            var speedrunScores = await Leaderboard.GetSpeedrunScoresEndless();
            var playerSpeedrunScore = await Leaderboard.GetPlayerSpeedrunScoreEndless();
            var playerInTop10Speedrun = scores.Exists(score => score.playerId == playerScore.playerId);
            if (!playerInTop10Speedrun)
            {
                speedrunScores[^1] = new SpeedrunScore(0, 0,  "---", "");
                speedrunScores[^2] = playerSpeedrunScore;
            }

            speedrunScores.ForEach(score =>
            {
                var time = TimeSpan.FromMilliseconds(score.ms).ToString(@"mm\:ss\.fff");
                templateSpeedrunLeaderboardEntryNameEndless.fontStyle = score.playerId == playerScore.playerId
                    ? FontStyles.Bold | FontStyles.Underline
                    : FontStyles.Normal;
                templateSpeedrunLeaderboardEntryRankEndless.text = score.rank != 0 ? score.rank.ToString() : "";
                templateSpeedrunLeaderboardEntryNameEndless.text = score.playerName.Split("#")[0];
                templateSpeedrunLeaderboardEntryTimeEndless.text = score.rank != 0 ? time : "";
                var obj = Instantiate(templateSpeedrunLeaderboardEntryEndless, speedrunLeaderBoardLayoutGroupEndless.transform, false);
                obj.SetActive(true);
            });
        }
        else if (isGameFinished)
        {
            isGameFinished = false;
            if (offline) return;
            await Leaderboard.AddTime();

            var scores = await Leaderboard.GetSpeedrunScores();
            var playerScore = await Leaderboard.GetPlayerSpeedrunScore();
            var playerInTop10 = scores.Exists(score => score.playerId == playerScore.playerId);
            if (!playerInTop10)
            {
                scores[^1] = new SpeedrunScore(0, 0,  "---", "");
                scores[^2] = playerScore;
            }

            scores.ForEach(score =>
            {
                var time = TimeSpan.FromMilliseconds(score.ms).ToString(@"mm\:ss\.fff");
                templateSpeedrunLeaderboardEntryName.fontStyle = score.playerId == playerScore.playerId
                    ? FontStyles.Bold | FontStyles.Underline
                    : FontStyles.Normal;
                templateSpeedrunLeaderboardEntryRank.text = score.rank != 0 ? score.rank.ToString() : "";
                templateSpeedrunLeaderboardEntryName.text = score.playerName.Split("#")[0];
                templateSpeedrunLeaderboardEntryTime.text = score.rank != 0 ? time : "";
                var obj = Instantiate(templateSpeedrunLeaderboardEntry, speedrunLeaderBoardLayoutGroup.transform, false);
                obj.SetActive(true);
            });
            progressModeTimeSinceStart = 0f;
            GameProgressController.ResetData();
        }
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
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        progressModeTimeSinceStart = 0f;
        
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

    public GameObject templateSpeedrunLeaderboardEntry;
    public TextMeshProUGUI templateSpeedrunLeaderboardEntryTime;
    public TextMeshProUGUI templateSpeedrunLeaderboardEntryName;
    public TextMeshProUGUI templateSpeedrunLeaderboardEntryRank;
    public GameObject speedrunLeaderBoardLayoutGroup;
    
    public GameObject finishGameScreen;
    public void FinishedGame()
    {
        SetIsPaused(false);
        SwitchToMenuMusic();
        finishGameScreen.SetActive(true);
        hud.SetActive(false);
        isGameFinished = true;
    }

    public void RestartGame()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        SetIsPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        SetIsPaused(false);
        SceneManager.LoadScene("MainMenu");
    }

    private async void SwitchToGameMusic()
    {
        gameMusic.Play();
        for (int i = 1; i <= 20; i++)
        {
            gameMusic.volume = i / 160f;
            menuMusic.volume = 1 - i / 20f;
            await Task.Delay(100);
        }

        if (!gameMusic.isPlaying)
        {
            gameMusic.volume = 1f;
            gameMusic.Play();
        }
        menuMusic.Stop();
    }
    private async void SwitchToMenuMusic()
    {
        menuMusic.Play();
        for (int i = 1; i <= 20; i++)
        {
            menuMusic.volume = i / 20f;
            gameMusic.volume = 0.125f - i / 160f;
            await Task.Delay(100);
        }
        
        if (!menuMusic.isPlaying)
        {
            menuMusic.volume = 1f;
            menuMusic.Play();
        }
        gameMusic.Stop();
    }
}
