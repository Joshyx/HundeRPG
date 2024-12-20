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
    public GameObject gameOverPanel;
    public Animator tongue;
    public SpriteRenderer biteTarget;
    Camera cam;
    PlayerMovement movement;
    
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
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
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameOverPanel.SetActive(true);
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
        npc.GetComponent<NPCController>().SpottedPlayer();
        tongue.SetTrigger("Lick");
    }

    public void MoveBiteTarget()
    {
        var pos = (Vector2) cam.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(pos, transform.position) > biteRadius)
        {
            pos = Vector2.MoveTowards(transform.position, pos, biteRadius);
        }
        biteTarget.transform.position = pos;
        
        var seconds = DateTime.Now.Subtract(biteStartTime.Value).TotalSeconds;
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
        if (npc is null)
        {
            return;
        }
        
        npc.GetComponent<NPCController>().TakeDamage(damage);
        StartCoroutine(nameof(MoveTowardsTargetSlowly), biteTarget.transform.position);
        AddXP(15f);
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
    
    IEnumerator MoveTowardsTargetSlowly(Vector3 target)
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 50f);
            yield return new WaitForEndOfFrame();
        }
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
        GameObject nearest = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, radius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("NPC"))
            {
                if (nearest is null || Vector2.Distance(pos, collider.transform.position) <
                    Vector2.Distance(pos, nearest.transform.position))
                {
                    nearest = collider.gameObject;
                }
            }
        }
        
        return nearest;
    }
}
