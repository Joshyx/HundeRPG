using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public MenuController menuController;
    public Animator tongue;
    public SpriteRenderer biteTarget;
    public GameObject landMine;
    private Camera cam;
    private PlayerMovement movement;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip lickSound;
    public AudioClip biteSound;
    public AudioClip moanSound;
    
    public float maxHealth = 100f;
    public float damage = 40f;
    [HideInInspector]
    public float currentDamage;
    public float lickRadius = 1.5f;
    [HideInInspector]
    public float lickDamage = 0f;
    public float secondsToLoadBite = 0.5f;
    public float biteRadius = 3.5f;
    public float distanceFromBiteTarget = 1f;
    public float lickCooldown = 0.5f;
    public float landMineCooldown = 1.5f;
    
    private float currentHealth;
    private DateTime? biteStartTime;
    private DateTime? lastLickTime;
    private DateTime? lastLandMineTime;

    private void Start()
    {
        currentHealth = maxHealth;
        currentDamage = damage;
        healthText.text = currentHealth.ToString();
        movement = GetComponent<PlayerMovement>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if(MenuController.IsGamePaused()) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Lick();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartPreparingBite();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopPreparingBite();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            MoveBiteTarget();
        }

        if (Input.GetKeyDown(KeyCode.Q) && GameProgressController.IsUpgradeEnabled("landmine"))
        {
            SpawnLandmine();
        }
    }

    public void TakeDamage(float amount)
    {
        if (GameProgressController.IsUpgradeEnabled("resistance"))
        {
            amount *= 0.8f;
        }
        AddHealth(-amount);
        AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
        menuController.GameOver();
    }

    public void Lick()
    {
        var secondsSinceLastLick = (DateTime.Now - lastLickTime)?.TotalSeconds ?? Double.MaxValue;
        if (secondsSinceLastLick <= lickCooldown)
        {
            return;
        }
        lastLickTime = DateTime.Now;
        
        tongue.SetTrigger("Lick");
        AudioSource.PlayClipAtPoint(lickSound, transform.position);
        
        var npc = Physics2D.OverlapCircle(transform.position, lickRadius, LayerMask.GetMask("NPC"))?.gameObject;

        if (npc is null)
        {
            return;
        }
        
        var controller = npc.GetComponent<NPCController>();
        var movement = npc.GetComponent<NPCMovement>();

        float xp = 5f;
        controller.SpottedPlayer();
        if (lickDamage > 0)
        {
            controller.TakeDamage(lickDamage);
            if (GameProgressController.IsUpgradeEnabled("lifesteal"))
            {
                AddHealth(lickDamage * 0.05f);
            }

            xp += 5f;
        }
        if (GameProgressController.IsUpgradeEnabled("cold_breath"))
        {
            movement.Freeze(8);
            xp += 5f;
        }
        GameProgressController.AddXP(xp);
        if (GameProgressController.IsUpgradeEnabled("knockback"))
        {
            movement.Knockback((npc.transform.position - transform.position).normalized * 10);
        }
    }


    public void MoveBiteTarget()
    {
        var pos = (Vector2) cam.ScreenToWorldPoint(Input.mousePosition);
        
        var biteTargetRadius = biteTarget.bounds.size.x / 2;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, pos - (Vector2) transform.position, biteRadius + biteTargetRadius, LayerMask.GetMask("Environment"));
        if (hit && !GameProgressController.IsUpgradeEnabled("wallbreaker"))
        {
            pos = hit.point + hit.normal * biteTargetRadius;
        } else
        {
            pos = Vector2.MoveTowards(transform.position, pos, biteRadius);
            var wall = Physics2D.OverlapCircle(pos, biteTargetRadius, LayerMask.GetMask("Environment"));
            if (wall)
            {
                pos = hit.point + hit.normal * biteTargetRadius;
            }
        }
        biteTarget.transform.position = pos;
        
        var seconds = DateTime.Now.Subtract(biteStartTime.GetValueOrDefault(DateTime.Now)).TotalSeconds;
        if (seconds >= secondsToLoadBite)
        {
            biteTarget.color = Color.black;
        }
    }
    public void StartPreparingBite()
    {
        biteStartTime = DateTime.Now;
        biteTarget.gameObject.SetActive(true);
        movement.DisableMovement();
    }
    public void StopPreparingBite()
    {
        if (biteStartTime == null)
        {
            return;
        }
        TimeSpan timeSpan = DateTime.Now.Subtract(biteStartTime.Value);
        biteStartTime = null;
        
        var seconds = timeSpan.TotalSeconds;
        if (seconds >= secondsToLoadBite)
        {
            Bite();
        }
        biteTarget.gameObject.SetActive(false);
        biteTarget.color = Color.black.WithAlpha(0.5f);
        movement.EnableMovement();
    }
    public void Bite()
    {
        StartCoroutine(nameof(MoveTowardsTargetSlowly), biteTarget.transform.position);
    }

    IEnumerator MoveTowardsTargetSlowly(Vector3 target)
    {
        boxCollider.enabled = false;
        if(GameProgressController.IsUpgradeEnabled("moaning_bite"))
        {
            AudioSource.PlayClipAtPoint(moanSound, transform.position);
        }
        else
        {
            AudioSource.PlayClipAtPoint(biteSound, transform.position);
        }
        while (Vector2.Distance(transform.position, target) > 0.5f)
        {
            rb.linearVelocity = (target - transform.position).normalized * 15;
            yield return new WaitForEndOfFrame();
        }
        boxCollider.enabled = true;
        rb.linearVelocity = Vector3.zero;
        List<Collider2D> npcs = new();
        if (GameProgressController.IsUpgradeEnabled("multiattack"))
        {
            npcs.AddRange(Physics2D.OverlapCircleAll(transform.position, distanceFromBiteTarget, LayerMask.GetMask("NPC")));
        }
        else
        {
            npcs.Add(Physics2D.OverlapCircle(transform.position, distanceFromBiteTarget, LayerMask.GetMask("NPC")));
        }
        
        float xp = 0f;
        foreach (var npc in npcs)
        {
            if (npc is not null)
            {
                var contr = npc.GetComponent<NPCController>();
                contr.TakeDamage(currentDamage);
                if (GameProgressController.IsUpgradeEnabled("lifesteal"))
                {
                    AddHealth(currentDamage * 0.05f);
                }

                if (GameProgressController.IsUpgradeEnabled("cold_breath"))
                {
                    npc.GetComponent<NPCMovement>().Freeze(10);
                    xp += 5f;
                }
                xp += contr.xpOnDamage;
            }
        }

        if (xp != 0)
        {
            GameProgressController.AddXP(xp);
        }
    }

    private void SpawnLandmine()
    {
        var secondsSinceLastLandMine = (DateTime.Now - lastLandMineTime)?.TotalSeconds ?? Double.MaxValue;
        if (secondsSinceLastLandMine <= landMineCooldown)
        {
            return;
        }
        
        lastLandMineTime = DateTime.Now;
        Instantiate(landMine, transform.position, Quaternion.identity);
    }

    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Min(Mathf.Max(currentHealth + amount, 0), maxHealth);
        healthText.SetText(Mathf.RoundToInt(currentHealth).ToString());
    }

    public void Consume(Consumable consumable)
    {
        StartCoroutine(nameof(ConsumeCoroutine), consumable);
    }
    private IEnumerator ConsumeCoroutine(Consumable consumable)
    {
        if (!Mathf.Approximately(consumable.damageMultiplier, 1f))
        {
            currentDamage *= consumable.damageMultiplier;
            yield return new WaitForSeconds(consumable.effectDurationSeconds);
            currentDamage = damage;
        }
        if (!Mathf.Approximately(consumable.speedMultiplier, 1f))
        {
            movement.currentSpeed *= consumable.speedMultiplier;
            yield return new WaitForSeconds(consumable.effectDurationSeconds);
            movement.currentSpeed = movement.runSpeed;
        }
        if (consumable.coins != 0)
        {
            GameProgressController.AddCoins(consumable.coins);
        }
        if (consumable.health != 0)
        {
            AddHealth(consumable.health);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lickRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, biteRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(biteTarget.transform.position, distanceFromBiteTarget);
    }
}
