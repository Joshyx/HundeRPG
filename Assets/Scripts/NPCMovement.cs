using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCMovement : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool canMove = true;
    private MovementState state = MovementState.IDLE;
    [HideInInspector]
    public Vector3 idleTargetPos;
    
    public float speed = 3;
    private bool frozen;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        idleTargetPos = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (MenuController.IsGamePaused())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        Move();
    }

    void Move() 
    {
        Vector3 playerPos = player.transform.position;
        Vector3 targetPos = idleTargetPos;
        
        switch (state)
        {
            case MovementState.IDLE:
                // Zur nächsten Zielposition laufen
                targetPos = idleTargetPos;
                break;
            case MovementState.ANGRY:
                // Zum Spieler laufen
                targetPos = playerPos;
                break;
            case MovementState.SCARED:
                // Vom Spieler weg laufen
                targetPos = transform.position + (-1 * (playerPos - transform.position));
                break;
        }
        
        bool looksRight = transform.position.x < targetPos.x;
        if (looksRight)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }

        if (!canMove)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        rb.linearVelocity = (targetPos - transform.position).normalized * (speed * (frozen ? 0.8f : 1));
        
        if (Vector2.Distance(transform.position, idleTargetPos) < 0.1f)
        {
            // Wenn die Zielposition erreicht wurde, eine zufällige neue aussuchen
            RecalculateIdlePosition();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (state != MovementState.IDLE) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Environment")) return;
        
        // Neue zufällige Zielposition weg von der Wand wenn eine Wand berührt wird
        idleTargetPos = (Vector2) transform.position + (other.GetContact(0).normal + Random.insideUnitCircle) * 10;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector3.zero;
    }

    public void EnableMovement()
    {
        canMove = true;
    }
    
    public bool CanMove() => canMove;

    private Coroutine lastUnfreezeRoutine;
    public void Freeze(float seconds)
    {
        frozen = true;
        StopCoroutine(lastUnfreezeRoutine);
        lastUnfreezeRoutine = StartCoroutine(nameof(UnfreezeAfter), seconds);
    }

    private IEnumerator UnfreezeAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        frozen = false;
    }

    public void RecalculateIdlePosition()
    {
        if (GameProgressController.IsUpgradeEnabled("cuteness"))
        {
            idleTargetPos = player.transform.position + (Vector3) Random.insideUnitCircle * 10;
        }
        else
        {
            idleTargetPos = new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10));
        }
    }

    public void SetState(MovementState newState)
    {
        state = newState;
    }

    public MovementState GetState()
    {
        return state;
    }
    
    public enum MovementState
    {
        IDLE, ANGRY, SCARED
    }
}
