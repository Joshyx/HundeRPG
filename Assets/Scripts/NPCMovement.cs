using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private GameObject player;

    private MovementState state = MovementState.IDLE;
    private Vector2 idleTargetPos;
    
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

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // Spieler aus dem Sichtfeld verschwunden
            // Weiter rumlaufen
            state = MovementState.IDLE;
        }
    }

    void Move() 
    {
        Vector2 playerPos = player.transform.position;
        
        switch (state)
        {
            case MovementState.IDLE:
                // Zur nächsten Zielposition laufen
                transform.position = Vector2.MoveTowards(transform.position, idleTargetPos, speed * Time.deltaTime);
                break;
            case MovementState.ANGRY:
                // Zum Spieler laufen
                transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
                break;
            case MovementState.SCARED:
                // Vom Spieler weg laufen
                transform.position = Vector2.MoveTowards(transform.position, playerPos, -1 * speed * Time.deltaTime);
                break;
        }
        
        if (Vector2.Distance(transform.position, idleTargetPos) < 0.1f)
        {
            // Wenn die Zielposition erreicht wurde, eine zufällige neue aussuchen
            idleTargetPos = new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10));
        }
    }

    public void SetState(MovementState newState)
    {
        state = newState;
    }
    
    public enum MovementState
    {
        IDLE, ANGRY, SCARED
    }
}