using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

public abstract class NPCController : MonoBehaviour
{
    public float maxHealth = 100f;
    public float viewDistance = 10f;
    public float attackDistance = 2f;
    
    public float damage = 10f;
    public float damageCooldownSeconds = 0.5f;
    private float? startOfAttack = null;
    
    private float currentHealth;

    protected PlayerController player;
    protected NPCMovement movement;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        movement = GetComponent<NPCMovement>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > viewDistance && movement.GetState() != NPCMovement.MovementState.IDLE)
        {
            LostPlayer();
        }

        TryAttack();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        SpottedPlayer();
    }

    private void TryAttack()
    {
        if (damage <= 0 ) return;

        if (Vector2.Distance(transform.position, player.transform.position) > attackDistance)
        {
            if (startOfAttack is not null)
            {
                startOfAttack = null;
                movement.EnableMovement();
            }
            return;
        }

        if (startOfAttack is null)
        {
            startOfAttack = Time.time;
            movement.DisableMovement();
            return;
        }
        
        var timeSinceStart = Time.time - startOfAttack.Value;
        if (timeSinceStart < damageCooldownSeconds)
        {
            return;
        }
        
        player.TakeDamage(damage);
        movement.EnableMovement();
        startOfAttack = null;
    }

    public abstract void SpottedPlayer();
    public abstract void LostPlayer();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
