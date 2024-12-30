using System;
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

    private static float[] xpNeededToLevelUp = { 10f, 10f, 10f, 10f, 10f };

    private static List<Upgrade> upgrades = new List<Upgrade>() {
        new (1, "Speed", "speed"),
        new (1, "Sharpened Tongue", "lick_damage"),
        new (1, "Long Tongue", "lick_distance"),
        new (1, "Damage", "attack_damage"),
        new (1, "Attack Distance", "attack_distance"),
        new (1, "Attack Cooldown", "attack_cooldown"),
        new (1, "Multiattack", "multiattack"),
        new (1, "Wallbreaker", "wall_breaker"),
        new (1, "TNTeeth", "explosive_teeth"),
        new (1, "Icecold Breath", "cold_breath"),
        new (1, "Lifesteal", "lifesteal"),
        new (1, "Landmine", "landmine"),
        new (1, "Moaning Bite", "moaning_bite"),
        new (1, "Cuteness", "cuteness"),
    };
    private static List<string> selectedUpgrades = new ();
    
    private static GameProgressController instance;
    
    public MenuController menuController;
    public TextMeshProUGUI levelText;
    public Slider xpSlider;
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
        instance.xpSlider.value = xp / (GetXPNeededToLevelUp() ?? xp);

        if (xp >= (GetXPNeededToLevelUp() ?? 10000000000))
        {
            LevelUp();
        }
    }

    private static void LevelUp()
    {
        level++;
        var availableUpgrades = upgrades
            .FindAll(upgrade => upgrade.minLevel <= level && !selectedUpgrades.Contains(upgrade.id))
            .OrderBy(_ => Random.value)
            .Take(3).ToList();
        
        instance.menuController.ShowLevelUpScreen(level, availableUpgrades, upgrade =>
        {
            selectedUpgrades.Add(upgrade.id);
            if (upgrade.id == "damage_damage")
            {
                instance.playerController.damage *= 2;
                instance.playerController.currentDamage = instance.playerController.damage;
            } else if (upgrade.id == "speed")
            {
                instance.playerMovement.runSpeed *= 2;
                instance.playerMovement.currentSpeed = instance.playerMovement.runSpeed;
            } else if (upgrade.id == "lick_damage")
            {
                instance.playerController.lickDamage = instance.playerController.damage / 3;
            } else if (upgrade.id == "lick_distance")
            {
                instance.playerController.lickRadius *= 2;
            } else if (upgrade.id == "attack_distance")
            {
                instance.playerController.biteRadius *= 2;
            } else if (upgrade.id == "attack_cooldown")
            {
                instance.playerController.biteRadius *= 0.5f;
            } else if (upgrade.id == "multiattack")
            {
                
            } else if (upgrade.id == "multiattack")
            {
                             
            } else if (upgrade.id == "multiattack")
            {
                                          
            } else if (upgrade.id == "multiattack")
            {
                                                       
            } else if (upgrade.id == "multiattack")
            {
                                                                    
            } else if (upgrade.id == "multiattack")
            {
                                                                                 
            } else if (upgrade.id == "multiattack")
            {
                                                                                              
            }
        });
        instance.levelText.text = GetXPNeededToLevelUp() is null ? "Max Level" : "Level " + level;
        xp = 0;
        instance.xpSlider.value = GetXPNeededToLevelUp() is null ? 1 : 0;
    }

    public static float? GetXPNeededToLevelUp()
    {
        try
        {
            return xpNeededToLevelUp[level];
        }
        catch
        {
            return null;
        }
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