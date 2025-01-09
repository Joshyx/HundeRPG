using System.Collections;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCMovement : MonoBehaviour
{
    private GameObject player;
    private SpriteRenderer sr;
    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;
    private GameObject target;
    private Rigidbody2D rb;

    private MovementState state = MovementState.IDLE;
    [HideInInspector]
    public Vector3 idleTargetPos;
    
    public float speed = 3;
    private bool frozen;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        aiPath.maxSpeed = speed;
        target = new GameObject(name + "_Move_Target");
        idleTargetPos = transform.position;
        destinationSetter.target = target.transform;
    }

    private bool inPause = false;
    // Update is called once per frame
    private void Update()
    {
        if (MenuController.IsGamePaused() && !inPause)
        {
            rb.linearVelocity = Vector2.zero;
            inPause = true;
            aiPath.canMove = false;
            return;
        }
        if (!MenuController.IsGamePaused() && inPause)
        {
            aiPath.canMove = true;
            inPause = false;
        }

        if (aiPath.velocity.magnitude < 0.1f)
        {
            var distance = Vector2.Distance(idleTargetPos, player.transform.position);
            var hit = Physics2D.Raycast(transform.position, idleTargetPos - transform.position, distance, LayerMask.GetMask("Environment", "NPC"));
            if (hit)
            {
                idleTargetPos = (Vector2) transform.position + (hit.normal + Random.insideUnitCircle) * 10;
            }
        }
        
        ChangeMoveTarget();
    }

    void ChangeMoveTarget() 
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

        target.transform.position = targetPos;
        
        if (Vector2.Distance(transform.position, idleTargetPos) < 0.1f)
        {
            // Wenn die Zielposition erreicht wurde, eine zufällige neue aussuchen
            RecalculateIdlePosition();
        }
    }

    public void DisableMovement()
    {
        aiPath.canMove = false;
    }

    public void EnableMovement()
    {
        aiPath.canMove = true;
    }
    
    public bool CanMove() => aiPath.canMove;

    private Coroutine lastUnfreezeRoutine;
    public void Freeze(float seconds)
    {
        frozen = true;
        aiPath.maxSpeed = speed * 0.8f;
        if (lastUnfreezeRoutine != null) StopCoroutine(lastUnfreezeRoutine);
        lastUnfreezeRoutine = StartCoroutine(nameof(UnfreezeAfter), seconds);
    }

    private IEnumerator UnfreezeAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        aiPath.maxSpeed = speed;
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
