using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    const string leaderboardKey = "HighestLevel";
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
        var password = name + "ASDsdf32@#&^";

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

    public static async Task<List<LeaderboardScore>> GetScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardKey, new GetScoresOptions
        {
            IncludeMetadata = true
        });
        var res = scoresResponse.Results.ConvertAll(entry =>
        {
            var metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(entry.Metadata);
            var level = int.Parse(metadata["level"]);
            var xp = float.Parse(metadata["xp"]);
            Debug.Log(entry.Metadata);
            return new LeaderboardScore(entry.Rank, level, xp, entry.PlayerName);
        });
        return res;
    }
}

public class LeaderboardScore
{
    public int rank;
    public int level;
    public float xp;
    public string playerName;

    public LeaderboardScore(int rank, int level, float xp, string playerName)
    {
        this.rank = rank;
        this.level = level;
        this.xp = xp;
        this.playerName = playerName;
    }
}