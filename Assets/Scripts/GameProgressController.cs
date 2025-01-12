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

    private static float[] xpNeededToLevelUp = { 140f, 150f, 150f, 160f, 180f, 200f, 240f, 280f, 350f };

    public List<Upgrade> upgrades;

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

    public static float GetTotalXP()
    {
        var previousXp = xpNeededToLevelUp.Take(level - 1).ToList().Sum();
        return previousXp + xp;
    }

    public static float GetXP() => xp;

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
        var availableUpgrades = instance.upgrades
            .FindAll(upgrade => upgrade.minLevel <= level && !selectedUpgrades.Contains(upgrade.id))
            .OrderBy(_ => Random.value)
            .Take(3).ToList();
        
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
            } else if (upgrade.id == "health")
            {
                instance.playerController.maxHealth *= 1.2f;
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

    private static float GetXPNeededToLevelUp() => GetXPNeededToLevelUp(level);
    private static float GetXPNeededToLevelUp(int lvl)
    {
        try
        {
            return xpNeededToLevelUp[lvl];
        }
        catch
        {
            return xpNeededToLevelUp.Last();
        }
    }
    public static int GetLevel() => level;

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

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public string id;
    public new string name;
    [TextArea]
    public string description;
    public Sprite icon;
    public int minLevel;
}