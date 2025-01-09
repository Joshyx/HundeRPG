using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class GameProgressController : MonoBehaviour
{
    private static int level = 1;
    private static float xp = 0;
    private static int coins = 0;
    private static int coinsNeededToProgress = 200;
    public AudioClip coinSound;
    public AudioClip xpSound;

    private static float[] xpNeededToLevelUp = { 100f, 100f, 120f, 140f, 160f, 180f, 220f, 280f, 350f };

    private static List<Upgrade> upgrades = new List<Upgrade>() {
        new (1, "Moaning Bite", "moaning_bite"),
        new (1, "Speed", "speed"),
        new (1, "Long Tongue", "lick_distance"),
        new (1, "Attack Distance", "attack_distance"),
        new (2, "Cuteness", "cuteness"),
        new (2, "Damage", "attack_damage"),
        new (3, "Icecold Breath", "cold_breath"),
        new (3, "Attack Cooldown", "attack_cooldown"),
        new (4, "Sharpened Tongue", "lick_damage"),
        new (4, "Multiattack", "multiattack"),
        new (5, "Wallbreaker", "wallbreaker"),
        new (6, "Lifesteal", "lifesteal"),
        new (6, "Landmine", "landmine"),
    };

    private static List<string> selectedUpgrades = new();
    
    private static GameProgressController instance;
    
    public MenuController menuController;
    public TextMeshProUGUI levelText;
    public Slider xpSlider;
    public TextMeshProUGUI coinsText;
    private PlayerMovement playerMovement;
    private PlayerController playerController;

    private void Start()
    {
        instance = this;
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerController = playerMovement.GetComponent<PlayerController>();
    }

    public static void AddXP(float xpToAdd)
    {
        xp += xpToAdd;
        instance.xpSlider.value = xp / GetXPNeededToLevelUp();
        AudioSource.PlayClipAtPoint(instance.xpSound, instance.playerMovement.transform.position);

        if (xp >= GetXPNeededToLevelUp())
        {
            instance.StartCoroutine(nameof(LevelUpAsSoonAsPossible));
        }
    }

    IEnumerator LevelUpAsSoonAsPossible()
    {
        while (MenuController.IsGamePaused())
        {
            yield return new WaitForSeconds(0.5f);
        }
        LevelUp();
    }

    private static void LevelUp()
    {
        level++;
        AddCoins(10);
        var availableUpgrades = upgrades
            .FindAll(upgrade => upgrade.minLevel <= level && !selectedUpgrades.Contains(upgrade.id))
            .OrderBy(_ => Random.value)
            .Take(3).ToList();

        if (availableUpgrades.Count == 0)
        {
            availableUpgrades.Add(new(0, "Continue", "continue"));
        }
        
        instance.menuController.ShowLevelUpScreen(level, availableUpgrades, upgrade =>
        {
            if(upgrade.id == "continue") return;
            
            selectedUpgrades.Add(upgrade.id);
            if (upgrade.id == "damage_damage")
            {
                instance.playerController.damage *= 1.2f;
                instance.playerController.currentDamage = instance.playerController.damage;
            } else if (upgrade.id == "speed")
            {
                instance.playerMovement.runSpeed *= 1.2f;
                instance.playerMovement.currentSpeed = instance.playerMovement.runSpeed;
            } else if (upgrade.id == "lick_damage")
            {
                instance.playerController.lickDamage = instance.playerController.damage / 3;
            } else if (upgrade.id == "lick_distance")
            {
                instance.playerController.lickRadius *= 1.5f;
            } else if (upgrade.id == "attack_distance")
            {
                instance.playerController.biteRadius *= 1.5f;
            } else if (upgrade.id == "attack_cooldown")
            {
                instance.playerController.secondsToLoadBite *= 0.7f;
            } else if (upgrade.id == "multiattack")
            {
                // PlayerController erkennt wenn Upgrade aktiviert ist, es muss also nicht hier programmiert werden
            } else if (upgrade.id == "wallbreaker")
            {
                // PlayerController erkennt wenn Upgrade aktiviert ist, es muss also nicht hier programmiert werden
            } else if (upgrade.id == "cold_breath")
            {
                // PlayerController erkennt wenn Upgrade aktiviert ist, es muss also nicht hier programmiert werden
            } else if (upgrade.id == "lifesteal")
            {
                // PlayerController erkennt wenn Upgrade aktiviert ist, es muss also nicht hier programmiert werden
            } else if (upgrade.id == "landmine")
            {
                // PlayerController erkennt wenn Upgrade aktiviert ist, es muss also nicht hier programmiert werden
            } else if (upgrade.id == "moaning_bite")
            {
                // PlayerController erkennt wenn Upgrade aktiviert ist, es muss also nicht hier programmiert werden
            } else if (upgrade.id == "cuteness")
            {
                // NPCMovement erkennt wenn Upgrade aktiviert ist, es muss also nicht hier programmiert werden
            }
        });
        instance.levelText.text = "Level " + level;
        xp = 0;
        instance.xpSlider.value = 0;
    }

    public static bool IsUpgradeEnabled(string upgradeId)
    {
        return selectedUpgrades.Contains(upgradeId);
    }

    public static float GetXPNeededToLevelUp()
    {
        try
        {
            return xpNeededToLevelUp[level];
        }
        catch
        {
            return xpNeededToLevelUp.Last();
        }
    }

    public static void AddCoins(int amount)
    {
        AudioSource.PlayClipAtPoint(instance.coinSound, instance.playerMovement.transform.position);
        coins += amount;
        instance.coinsText.text = coins.ToString();
    }

    public static bool CanProgress()
    {
        return coins >= coinsNeededToProgress;
    }
}

public class Upgrade
{
    public int minLevel;
    public string name;
    public string id;

    public Upgrade(int minLevel, string name, string id)
    {
        this.minLevel = minLevel;
        this.name = name;
        this.id = id;
    }
}