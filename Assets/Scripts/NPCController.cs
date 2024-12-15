using System;
using UnityEngine;

public abstract class NPCController : MonoBehaviour
{
    public float maxHealth = 100f;
    public float viewDistance = 10f;
    
    public float damage = 10f;
    public float damageCooldownSeconds = 1f;
    
    private float currentHealth;
    private float timeSinceLastDamage = 10000f;

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
        timeSinceLastDamage += Time.deltaTime;
        
        if (Vector2.Distance(transform.position, player.transform.position) > viewDistance && movement.GetState() != NPCMovement.MovementState.IDLE)
        {
            LostPlayer();
        }
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

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player") || timeSinceLastDamage < damageCooldownSeconds)
        {
            return;
        }
        
        player.TakeDamage(damage);
        timeSinceLastDamage = 0;
    }

    public abstract void SpottedPlayer();
    public abstract void LostPlayer();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
