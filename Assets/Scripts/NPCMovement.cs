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
    private Animator anim;
    private ParticleSystem walkParticles;

    private MovementState state = MovementState.IDLE;
    [HideInInspector]
    public Vector3 idleTargetPos;
    
    public float speed = 3;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        aiPath.maxSpeed = speed;
        target = new GameObject(name + "_Move_Target");
        idleTargetPos = transform.position;
        destinationSetter.target = target.transform;
        anim.SetBool("isRunning", true);
        walkParticles = GetComponentInChildren<ParticleSystem>();
    }

    private bool inPause = false;
    // Update is called once per frame
    private void Update()
    {
        if (MenuController.IsGamePaused())
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
        anim.SetBool("isRunning", CanMove());

        if (aiPath.velocity.magnitude < 0.1f && state == MovementState.IDLE)
        {
            var distance = Vector2.Distance(idleTargetPos, player.transform.position);
            var hit = Physics2D.Raycast(transform.position, idleTargetPos - transform.position, distance, LayerMask.GetMask("Environment", "NPC"));
            if (hit)
            {
                idleTargetPos = (Vector2) transform.position + (hit.normal + Random.insideUnitCircle) * 10;
            }
        }
        
        ChangeMoveTarget();
        Flip();
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

        target.transform.position = targetPos;
        
        if (Vector2.Distance(transform.position, idleTargetPos) < 0.1f)
        {
            // Wenn die Zielposition erreicht wurde, eine zufällige neue aussuchen
            RecalculateIdlePosition();
        }
    }

    private void Flip()
    {
        var targetX = CanMove() ? target.transform.position.x : player.transform.position.x;
        var looksRight = transform.position.x < targetX;
        sr.flipX = looksRight;
    }

    public void DisableMovement()
    {
        rb.linearVelocity = Vector2.zero;
        aiPath.canMove = false;
        anim.SetBool("isRunning", false);
        walkParticles.Stop();
    }

    public void EnableMovement()
    {
        if (inKnockback) return;
        
        aiPath.canMove = true;
        anim.SetBool("isRunning", true);
        walkParticles.Play();
    }

    private bool inKnockback;
    public void Knockback(Vector2 direction)
    {
        StopAllCoroutines();
        GetComponent<NPCController>().StopAllCoroutines();
        inKnockback = true;
        DisableMovement();
        rb.linearVelocity = direction;
        StartCoroutine(nameof(EndKnockback));
    }
    IEnumerator EndKnockback()
    {
        yield return new WaitForSeconds(0.7f);
        inKnockback = false;
        rb.linearVelocity = Vector2.zero;
        EnableMovement();
    }
    
    public bool CanMove() => aiPath.canMove;

    private Coroutine lastUnfreezeRoutine;
    public void Freeze(float seconds)
    {
        aiPath.maxSpeed = speed * 0.8f;
        if (lastUnfreezeRoutine != null) StopCoroutine(lastUnfreezeRoutine);
        lastUnfreezeRoutine = StartCoroutine(nameof(UnfreezeAfter), seconds);
    }

    private IEnumerator UnfreezeAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        aiPath.maxSpeed = speed;
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
