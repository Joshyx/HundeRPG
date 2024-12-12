using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Animator tongue;
    
    public float maxHealth = 100f;
    public float lickRadius = 1.5f;
    
    private float currentHealth;
    private float level;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Lick();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log("DEAD");
        }
    }

    public void Lick()
    {
        var npc = GetNearestNPCInRadius(lickRadius);

        if (npc is null)
        {
            Debug.Log("NO NPC");
            return;
        }
        
        level += 0.1f;
        levelText.SetText("Level: " + level);
        npc.GetComponent<NPCController>().SpottedPlayer();
        tongue.SetTrigger("Lick");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lickRadius);
    }

    private GameObject GetNearestNPCInRadius(float radius)
    {
        GameObject nearest = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("NPC"))
            {
                if (nearest is null || Vector2.Distance(transform.position, collider.transform.position) <
                    Vector2.Distance(transform.position, nearest.transform.position))
                {
                    nearest = collider.gameObject;
                }
            }
        }
        
        return nearest;
    }
}
