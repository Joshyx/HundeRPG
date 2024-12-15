using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private GameObject player;

    private MovementState state = MovementState.IDLE;
    [HideInInspector]
    public Vector2 idleTargetPos;
    
    public float speed = 3;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        idleTargetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move() 
    {
        Vector2 playerPos = player.transform.position;
        bool looksRight = false;
        
        switch (state)
        {
            case MovementState.IDLE:
                if (transform.position.x < idleTargetPos.x)
                {
                    looksRight = true;
                }
                // Zur nächsten Zielposition laufen
                transform.position = Vector2.MoveTowards(transform.position, idleTargetPos, speed * Time.deltaTime);
                break;
            case MovementState.ANGRY:
                if (transform.position.x < playerPos.x)
                {
                    looksRight = true;
                }
                // Zum Spieler laufen
                transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
                break;
            case MovementState.SCARED:
                if (transform.position.x > playerPos.x)
                {
                    looksRight = true;
                }
                // Vom Spieler weg laufen
                transform.position = Vector2.MoveTowards(transform.position, playerPos, -1 * speed * Time.deltaTime);
                break;
        }

        if (looksRight)
        {
            transform.localScale = new Vector3(-1* Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        
        if (Vector2.Distance(transform.position, idleTargetPos) < 0.1f)
        {
            // Wenn die Zielposition erreicht wurde, eine zufällige neue aussuchen
            RecalculateIdlePosition();
        }
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
