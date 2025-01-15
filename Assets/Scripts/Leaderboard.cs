using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    private const string leaderboardKey = "leaderboard";
    private const string speedrunLeaderboardKeyEndless = "speedrunEndless";
    private const string totalTimeSpeedrunLeaderboardKey = "speedrun";
    private static string playerName;
    
    private async void Start()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }
    }

    public static async Task Login(string name)
    {
        if (AuthenticationService.Instance.IsSignedIn) return;
        var password = "ASDsdf32@#&^";

        AuthenticationService.Instance.SignedIn += async () =>
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
        };
        AuthenticationService.Instance.SignInFailed += (e) =>
        {
            Debug.LogError(e.Message);
        };

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(name, password);
        }
        catch
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(name, password);
        }
    }
    public static async Task AddScore()
    {
        var totalXp = GameProgressController.GetTotalXP();
        var xp = GameProgressController.GetXP();
        var level = GameProgressController.GetLevel();

        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardKey, totalXp, new AddPlayerScoreOptions
        {
            Metadata = new Dictionary<string, string>
            {
                {"xp", xp.ToString()},
                {"level", level.ToString()},
            }
        });
    }

    public static async Task AddTimeEndless()
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(speedrunLeaderboardKeyEndless, MenuController.progressModeTimeSinceStart);
    }
    public static async Task AddTime()
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(totalTimeSpeedrunLeaderboardKey, MenuController.progressModeTimeSinceStart);
    }

    public static async Task<List<LeaderboardScore>> GetScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardKey, new GetScoresOptions
        {
            IncludeMetadata = true
        });
        var res = scoresResponse.Results.ConvertAll(EntryToLeaderboardScore);
        return res;
    }

    public static async Task<List<SpeedrunScore>> GetSpeedrunScoresEndless()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(speedrunLeaderboardKeyEndless);
        var res = scoresResponse.Results.ConvertAll(EntryToSpeedrunScore);
        return res;
    }
    public static async Task<List<SpeedrunScore>> GetSpeedrunScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(totalTimeSpeedrunLeaderboardKey);
        var res = scoresResponse.Results.ConvertAll(EntryToSpeedrunScore);
        return res;
    }

    public static async Task<LeaderboardScore> GetPlayerScore()
    {
        var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardKey, new GetPlayerScoreOptions()
        {
            IncludeMetadata = true
        });
        return EntryToLeaderboardScore(entry);
    }

    public static async Task<SpeedrunScore> GetPlayerSpeedrunScoreEndless()
    {
        var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(speedrunLeaderboardKeyEndless);
        return EntryToSpeedrunScore(entry);
    }
    public static async Task<SpeedrunScore> GetPlayerSpeedrunScore()
    {
        var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(totalTimeSpeedrunLeaderboardKey);
        return EntryToSpeedrunScore(entry);
    }

    private static LeaderboardScore EntryToLeaderboardScore(LeaderboardEntry entry)
    {
        var metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(entry.Metadata);
        var level = int.Parse(metadata["level"]);
        var xp = float.Parse(metadata["xp"]);
        return new LeaderboardScore(entry.Rank + 1, level, xp, entry.PlayerName, entry.PlayerId);
    }

    private static SpeedrunScore EntryToSpeedrunScore(LeaderboardEntry entry)
    {
        return new SpeedrunScore(entry.Rank + 1, Mathf.RoundToInt((float) entry.Score), entry.PlayerName, entry.PlayerId);
    }
}

public class LeaderboardScore
{
    public int rank;
    public int level;
    public float xp;
    public string playerName;
    public string playerId;

    public LeaderboardScore(int rank, int level, float xp, string playerName, string playerId)
    {
        this.rank = rank;
        this.level = level;
        this.xp = xp;
        this.playerName = playerName;
        this.playerId = playerId;
    }
}

public class SpeedrunScore
{
    public int rank;
    public int ms;
    public string playerName;
    public string playerId;
    
    public SpeedrunScore(int rank, int ms, string playerName, string playerId)
    {
        this.rank = rank;
        this.ms = ms;
        this.playerName = playerName;
        this.playerId = playerId;
    }
}
