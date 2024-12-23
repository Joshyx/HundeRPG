using System;
using System.Collections;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Slider levelSlider;
    public TextMeshProUGUI healthText;
    public MenuController menuController;
    public Animator tongue;
    public SpriteRenderer biteTarget;
    private Camera cam;
    private PlayerMovement movement;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip lickSound;
    public AudioClip biteSound;
    
    public float maxHealth = 100f;
    public float damage = 40f;
    public float lickRadius = 1.5f;
    public float secondsToLoadBite = 0.5f;
    public float biteRadius = 3.5f;
    public float distanceFromBiteTarget = 1f;
    public float lickCooldown = 0.5f;
    
    private float currentHealth;
    private float xp;
    private float xpNeededToLevelUp = 100;
    private DateTime? biteStartTime;
    private DateTime? lastLickTime;

    private void Start()
    {
        currentHealth = maxHealth;
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
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthText.SetText(Mathf.Max(Mathf.RoundToInt(currentHealth), 0).ToString());
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
        var npc = GetNearestNPCInRadius(transform.position, lickRadius);

        if (npc is null)
        {
            return;
        }
        
        AddXP(5f);
        AudioSource.PlayClipAtPoint(lickSound, transform.position);
        npc.GetComponent<NPCController>().SpottedPlayer();
        tongue.SetTrigger("Lick");
    }

    public void MoveBiteTarget()
    {
        var pos = (Vector2) cam.ScreenToWorldPoint(Input.mousePosition);
        
        var biteTargetRadius = biteTarget.bounds.size.x / 2;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, pos - (Vector2) transform.position, biteRadius + biteTargetRadius, LayerMask.GetMask("Environment"));
        if (hit)
        {
            pos = hit.point + hit.normal * biteTargetRadius;
        } else if (Vector2.Distance(pos, transform.position) > biteRadius)
        {
            pos = Vector2.MoveTowards(transform.position, pos, biteRadius);
        }
        biteTarget.transform.position = pos;
        
        var seconds = DateTime.Now.Subtract(biteStartTime.GetValueOrDefault(DateTime.Now)).TotalSeconds;
        if (seconds >= secondsToLoadBite)
        {
            biteTarget.color = Color.red;
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
        biteTarget.color = Color.blue;
        movement.EnableMovement();
    }
    public void Bite()
    {
        var npc = GetNearestNPCInRadius(biteTarget.transform.position, distanceFromBiteTarget);
        if (npc is not null)
        {
            npc.GetComponent<NPCController>().TakeDamage(damage);
            AddXP(15f);
        }
        
        AudioSource.PlayClipAtPoint(biteSound, transform.position);
        StartCoroutine(nameof(MoveTowardsTargetSlowly), biteTarget.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var consumable = other.GetComponent<Consumable>();
        if (consumable is null) return;

        if (!Mathf.Approximately(consumable.damageMultiplier, 1f))
        {
            StartCoroutine(nameof(IncreaseDamageTemporarily), new Tuple<float, float>(consumable.damageMultiplier, consumable.effectDurationSeconds));
        }
        if (!Mathf.Approximately(consumable.speedMultiplier, 1f))
        {
            StartCoroutine(nameof(IncreaseSpeedTemporarily), new Tuple<float, float>(consumable.speedMultiplier, consumable.effectDurationSeconds));
        }
        Destroy(consumable.gameObject);
    }

    private IEnumerator IncreaseSpeedTemporarily(Tuple<float, float> args)
    {
        var oldSpeed = movement.runSpeed;
        movement.runSpeed *= args.Item1;
        yield return new WaitForSeconds(args.Item2);
        movement.runSpeed = oldSpeed;
    }

    private IEnumerator IncreaseDamageTemporarily(Tuple<float, float> args)
    {
        var oldDamage = damage;
        damage *= args.Item1;
        yield return new WaitForSeconds(args.Item2);
        damage = oldDamage;
    }

    void AddXP(float amount)
    {
        xp += amount;
        levelSlider.value = xp / xpNeededToLevelUp;
    }

    public bool CanLevelUp()
    {
        return xp >= xpNeededToLevelUp;
    }

    public float GetProgress()
    {
        return xp / xpNeededToLevelUp;
    }
    
    IEnumerator MoveTowardsTargetSlowly(Vector3 target)
    {
        boxCollider.enabled = false;
        while (Vector2.Distance(transform.position, target) > 0.5f)
        {
            rb.linearVelocity = (target - transform.position).normalized * 15;
            yield return new WaitForEndOfFrame();
        }
        boxCollider.enabled = true;
        rb.linearVelocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lickRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, biteRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(biteTarget.transform.position, distanceFromBiteTarget);
    }

    private GameObject GetNearestNPCInRadius(Vector2 pos, float radius)
    {
        Collider2D collider = Physics2D.OverlapCircle(pos, radius, LayerMask.GetMask("NPC"));
        
        return collider ? collider.gameObject : null;
    }
}
