using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCMovement : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;

    private bool canMove = true;
    private MovementState state = MovementState.IDLE;
    [HideInInspector]
    public Vector3 idleTargetPos;
    
    public float speed = 3;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        idleTargetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
            transform.localScale = new Vector3(-1* Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (!canMove)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        rb.linearVelocity = (targetPos - transform.position).normalized * speed;
        
        if (Vector2.Distance(transform.position, idleTargetPos) < 0.1f)
        {
            // Wenn die Zielposition erreicht wurde, eine zufällige neue aussuchen
            RecalculateIdlePosition();
        }
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

    public void RecalculateIdlePosition()
    {
        idleTargetPos = new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10));
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
