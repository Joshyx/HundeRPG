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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (damage <= 0 ) return;
        
        if (!other.gameObject.CompareTag("Player")) return;

        StartCoroutine(nameof(AttackPlayer));
    }

    private IEnumerator AttackPlayer()
    {
        movement.DisableMovement();
        yield return new WaitForSeconds(damageCooldownSeconds);
        if (Vector2.Distance(transform.position, player.transform.position) < attackDistance)
        {
            player.TakeDamage(damage);
        }
        movement.EnableMovement();
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
